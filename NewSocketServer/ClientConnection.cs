using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using log4net.Core;
using log4net;

namespace SocketServer
{
	public class ClientConnection
	{
		#region Fields
		private static readonly ILog logger = LogManager.GetLogger(typeof(ClientConnection));
		private readonly object sendLock = new object();

		private MessageDispatcher messageHandler;
		private Thread responderThread;

		private Exception currentException;

		private volatile bool running;
		private volatile IMessage sendResponse;

		private readonly AutoResetEvent sendResponded = new AutoResetEvent(false);
		private readonly AutoResetEvent messageSent = new AutoResetEvent(false);

		#endregion
		
		#region Constructors 
		/// <summary>
		/// Creates a controller channel with a default TCP transport
		/// </summary>
		public ClientConnection()
		{
			Initialize(new SocketTransport(), new PlainEnvelope(), new MessageDispatcher());
		}

		/// <summary>
		/// Creates a controller channel with the given transport
		/// </summary>
		/// <param name="transport">The transport for the channel to communicate over</param>
		public ClientConnection(INetworkTransport transport)
		{
			Initialize(transport, new PlainEnvelope(), new MessageDispatcher());
		}

		/// <summary>
		/// Creates a controller channel with a default TCP transport that is able to handle incoming messages
		/// using the given dispatcher
		/// </summary>
		/// <param name="dispatcher">A MessageDispatcher to route messages to appropriate handlers</param>
		public ClientConnection(MessageDispatcher dispatcher)
		{
			Initialize(new SocketTransport(), new PlainEnvelope(), dispatcher);
		}

		/// <summary>
		/// Creates a controller channel with a default TCP transport that is able to handle incoming messages
		/// using the given dispatcher
		/// </summary>
		/// <param name="transport">A transport used to send/receive messages</param>
		/// <param name="dispatcher">A MessageDispatcher to route messages to appropriate handlers</param>
		public ClientConnection(INetworkTransport transport, MessageDispatcher dispatcher)
		{
			Initialize(transport, new PlainEnvelope(), dispatcher);
		}

		/// <summary>
		/// Creates a controller channel with a transport that is able to handle incoming messages
		/// using the given envelope and dispatcher
		/// </summary>
		/// <param name="transport">A transport used to send/receive messages</param>
		/// <param name="envelope"></param>
		/// <param name="dispatcher">A MessageDispatcher to route messages to appropriate handlers</param>
		public ClientConnection(INetworkTransport transport, Envelope envelope, MessageDispatcher dispatcher)
		{
			Initialize(transport, envelope, dispatcher);
		}


		#endregion

		#region Factory Methods

		/// <summary>
		/// Creates a controller channel with the given transport, dispatcher, and ControlMode
		/// This constructor is used only by the ChannelListener when creating new server channels
		/// </summary>
		/// <param name="transport">The transport for the channel to communicate over</param>
		/// <param name="envelope"></param>
		/// <param name="dispatcher">A MessageDispatcher to route messages to appropriate handlers</param>
		/// <param name="protocol"></param>
		public static ClientConnection CreateListenerChannel(INetworkTransport transport, Envelope envelope, MessageDispatcher dispatcher)
		{
			ClientConnection channel = new ClientConnection(transport, envelope, dispatcher);

			channel.StartReceiveThread();
			return channel;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether the channel is connected to the remote endpoint
		/// </summary>
		public bool IsConnected
		{
			get { return Transport.IsConnected; }
		}
		/// <summary>
		/// Gets or sets a unique identifier for this channel.  This is not initially the same identifier
		/// as the Channel on the other side of the connection.  Identifiers are synchronized only when
		/// a controller channel changes its mode to become a responder.
		/// </summary>
		public Guid ConnectionID { get; set; }

		/// <summary>
		/// Allows access to the message dispatcher for this channel in order to register new message handlers
		/// </summary>
		public MessageDispatcher Dispatcher { get; private set; }
		
		/// <summary>
		/// Gets the remote endpoint of this connection.
		/// </summary>
		public EndPoint EndPoint { get { return Transport.RemoteEndPoint; } }

		public int MessageResponseTimeout { get; set; }        

		public INetworkTransport Transport { get; private set; }

		public Envelope Envelope { get; private set; }

		public string Name { get; set; }

		#endregion

		/// <summary>
		/// All messages sent should pass through this method in order to get
		/// the raw message data and trigger the corresponding event.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="outStream"></param>
		private void Serialize(IMessage message, Stream outStream)
		{
			using (StreamWrapper wrapper = new StreamWrapper(outStream))
			{
				Envelope.Serialize(message, wrapper);
				RaiseMessageSent(message, wrapper.GetOutputBytes());
			} 
		}

		/// <summary>
		/// All messages should be read with this method in order to get
		/// the raw message data and trigger the corresponding event.
		/// </summary>
		/// <param name="inStream"></param>
		/// <returns></returns>
		private IMessage Deserialize(Stream inStream)
		{
			using (StreamWrapper wrapper = new StreamWrapper(inStream))
			{
				IMessage message = Envelope.Deserialize(wrapper);
				RaiseMessageReceived(message, wrapper.GetInputBytes());
				return message;
			}
		}

		private void Initialize(INetworkTransport trans, Envelope envelope, MessageDispatcher disp)
		{
			ConnectionID = Guid.NewGuid();
			MessageResponseTimeout = Timeout.Infinite;
			Transport = trans;
			Envelope = envelope;
			Dispatcher = disp;
		}
		/// <summary>
		/// Starts a thread for Channels in Responder mode to process and handle incoming messages
		/// </summary>
		internal void StartReceiveThread()
		{
			if (running) return;

			responderThread = new Thread(ProcessMessages) {Name = "ClientConnectionThread", IsBackground = true};
			responderThread.Start();
		}

		internal void StopReceiveThread()
		{
			running = false;
			Pulse();
			// This method is called by the responder thread in response to certain protocol message, like QUIT.
			// Because of this, we can't join our own thread to make sure it's halted.  
			// It's up to the caller of this method to decide whether they need to join the receiveThread
			// in order to ensure proper shutdown.
		}

		/// <summary>
		/// Connects this channel to the remote address and port.
		/// </summary>
		/// <param name="serverAddress">The server address to connect to.</param>
		/// <param name="port">The server port to connect to.</param>
		/// <returns>A Connected message providing information about the remote server.</returns>
		public void Connect(string serverAddress, int port)
		{
			lock (sendLock)
			{
				// reset this so that a reused channel will wait for message responses
				sendResponded.Reset();
				sendResponse = null;

				Transport.Address = serverAddress;
				Transport.Port = port;
				Transport.Connect();

				StartReceiveThread();
			}
		}

		public void Connect()
		{
			Connect(Transport.Address, Transport.Port);
		}

		/// <summary>
		/// Shuts down the connection.
		/// </summary>
		public void Disconnect()
		{
			Disconnect(false);
		}

		/// <summary>
		/// Shuts down the connection.
		/// </summary>
		/// <param name="forceDisconnect"> <c>true</c> [forceDisconnect].</param>
		public void Disconnect(bool forceDisconnect)
		{
			//lock (syncSend)
			{
				StopReceiveThread();

				if (Transport == null)
					logger.ErrorFormat("Channel '{0}' transport is null in Disconnect", Name);
				else
				{
					Transport.Disconnect(forceDisconnect);
					bool joined = responderThread.Join(5000);
					// If we can't join the receive thread, disconnect the transport forcefully
					if (!joined)
						Transport.Disconnect(true);
				}

				RaiseChannelClosed(null);
			}
		}

		private void ProcessMessages()
		{
			try
			{
				running = true;
				
				while (running)
				{
					ProcessAsynchronousMessages();
				}
			}
			catch (InvalidOperationException ex)
			{
				LogChannelError(ex, Level.Finer);
			}
			catch (IOException ex)
			{
				LogChannelError(ex, Level.Finer);
			}
			catch (Exception ex)
			{
				LogChannelError(ex, Level.Error);
			}
			finally
			{
				running = false;
				if (Transport.IsConnected)
				{
					Transport.Disconnect(true);
				}
				RaiseChannelClosed(currentException);
				sendResponded.Set(); // make sure no synchronous sends are still waiting for a response that will never come.
			}
		}

		private void LogChannelError(Exception ex, Level level)
		{
			currentException = ex;

			logger.Logger.Log(null, level, "Message processing error on channel '" + Name + "': " + ex.Message, ex);
			if (ex.InnerException != null)
				logger.Logger.Log(null, level, "InnerException:\n" + ex.InnerException.Message, ex.InnerException);           
		}

		private void ProcessAsynchronousMessages()
		{
			IMessage response = null;
			IMessage message = null;
			try
			{
				message = Deserialize(Transport.Stream);
				message.Validate();
				response = Dispatcher.DispatchMessage(message, this);
			}
			//catch (UnknownMessageException ex)
			//{
			//    response = Protocol.ProcessUnknownMessage(ex.Message);
			//}
			//catch (ProtocolException ex)
			//{
			//    response = Protocol.ProcessProtocolException(ex.Message);
			//}
			//catch (InvalidMessageException ex)
			//{
			//    response = Protocol.ProcessInvalidMessageException(ex.InvalidMessage, ex.Message);
			//}
			//catch (UnhandledMessageException ex)
			//{
			//    response = Protocol.ProcessUnhandledMessage(ex.UnhandledMessage);
			//}
			//catch (MessageProcessingException ex)
			//{
			//    response = Protocol.ProcessHandlerException(message, ex.Message);
			//}
			catch (Exception ex)
			{

			}
			
			if (response != null)
				Serialize(response, Transport.Stream);
		}

		/// <summary>
		/// Causes the receiver thread to break out of its wait handle to process messages or to quit.
		/// </summary>
		internal void Pulse()
		{
			messageSent.Set();
		}


		#region Public Events
		/// <summary>
		/// Event Raised whenever an incoming message is received
		/// </summary>
		public event EventHandler<MessageEventArgs> MessageReceived;

		/// <summary>
		/// Event Raised whenever an outgoing message is sent
		/// </summary>
		public event EventHandler<MessageEventArgs> MessageSent;

		/// <summary>
		/// Event raised when this channel is closed from the remote endpoint
		/// </summary>
		public event EventHandler<ChannelEventArgs> ChannelClosed;

		#endregion

		#region Event Raisers
		private void RaiseMessageSent(IMessage message, byte[] raw)
		{
			var handler = MessageSent;
			if (handler != null)
				handler(this, new MessageEventArgs(this, message, raw));
		}

		private void RaiseMessageReceived(IMessage message, byte[] raw)
		{
			var handler = MessageReceived;
			if (handler != null)
				handler(this, new MessageEventArgs(this, message, raw));
		}

		private void RaiseChannelClosed(Exception exception)
		{
			var handler = ChannelClosed;
			if (handler != null)
				handler(this, new ChannelEventArgs(this, exception));
		}

		#endregion

		/// <summary>
		/// Used by channel controllers to send a message synchronously to the remote endpoint and get a response
		/// </summary>
		/// <param name="message"></param>
		public T Send<T>(IMessage message) where T : IMessage
		{
			lock (sendLock)
			{
				if (message == null)
					throw new NullReferenceException("Attempt to send a null message");

				if (!typeof (T).IsAbstract && !typeof (T).IsInterface)
					MessageFactory.Register(typeof (T));

				Serialize(message, Transport.Stream);
				messageSent.Set();

				string endpoint = Transport.RemoteEndPoint == null ? "Unknown" : Transport.RemoteEndPoint.ToString();
				logger.InfoFormat("Sent message {0} to {1}", message.Name, endpoint);

				// wait for a response
				bool responded = sendResponded.WaitOne(MessageResponseTimeout, false);
				if (!responded) throw new TimeoutException("Timeout waiting for message response");

				IMessage response = sendResponse;
				sendResponse = null;

				if (response is T)
					return (T)response;

				//// Let the protocol have a chance at handling an error response.  
				//string errorString = Protocol.IsError(response);
				//if (errorString != null)
				//	throw new ProtocolException(errorString);

				// If this is not an error message, we need to 
				// inform the caller that we did not receive what it wanted.
				string messageName = response != null ? response.Name : "(no message)";
				throw new ProtocolException(string.Format("Unexpected Message {0} received in response to {1}", messageName, message.Name));
			}
		}

		#region IMessageSender Members

		/// <summary>
		/// Send a message asynchronously, ignoring any response.
		/// This method will not throw a protocol exception if an error occurs on the server.
		/// This method should be used when a large number of commands need to be sent together.
		/// </summary>
		/// <param name="message"></param>
		public void Send(IMessage message)
		{
			lock (sendLock)
			{
				if (message == null)
					throw new NullReferenceException("Attempt to send a null message");


				//Interlocked.Increment(ref ignoreResponses);
				Serialize(message, Transport.Stream);
				messageSent.Set();
				logger.InfoFormat("Sent message {0}", message.Name);
			}
		}

		#endregion

		public static T Send<T>(string server, int port, int txRxTimeout, IMessage message) where T : IMessage
		{
			SocketTransport transport = new SocketTransport { SendTimeout = txRxTimeout, ReceiveTimeout = txRxTimeout };
			ClientConnection connection = new ClientConnection(transport);
			connection.Connect(server, port);
			T response = connection.Send<T>(message);
			connection.Disconnect();
			return response;
		}

		public static T Send<T>(string server, int port, IMessage message) where T : IMessage
		{
			ClientConnection connection = new ClientConnection();
			connection.Connect(server, port);
			T response = connection.Send<T>(message);
			connection.Disconnect();
			return response;
		}

		public static void Send(string server, int port, IMessage message)
		{
			ClientConnection connection = new ClientConnection();
			connection.Connect(server, port);
			connection.Send(message);
			connection.Disconnect();
		}
	}
}

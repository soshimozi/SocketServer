using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using log4net.Core;
using log4net;
using TestServer.Event;
using TestServer.Messaging;

namespace TestServer.Network
{
    public class ClientConnection
    {
        private static ILog Logger = LogManager.GetLogger(typeof(ClientConnection));

        private volatile bool running = false;

        private Thread responderThread = null;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ClientEventArgs> ClientClosed;

        public Envelope Envelope { get; private set; }

        public static ClientConnection CreateClientConnection(INetworkTransport transport, Envelope envelope)
        {
            ClientConnection channel = new ClientConnection(transport, envelope);

            channel.StartMessagePump();
            return channel;
        }

        public ClientConnection()
            : this(new SocketTransport(), new PlainEnvelope())
        {
        }

        public ClientConnection(INetworkTransport transport)
            : this(transport, new PlainEnvelope())
        {
        }

        public ClientConnection(INetworkTransport transport, Envelope envelope)
        {
            Transport = transport;
            Envelope = envelope;

            ConnectionID = Guid.NewGuid();
        }

        public Guid ConnectionID { get; set; }

        public bool IsConnected
        {
            get { return Transport.IsConnected; }
        }

        public void Connect(string serverAddress, int port)
        {
            Transport.Address = serverAddress;
            Transport.Port = port;
            Transport.Connect();

            StartMessagePump();
        }

        /// <summary>
        /// Gets the remote endpoint of this connection.
        /// </summary>
        public EndPoint EndPoint { get { return Transport.RemoteEndPoint; } }

        public INetworkTransport Transport { get; private set; }

        public void StartMessagePump()
        {
            if (running) return;

            responderThread = new Thread(MessagePump) { Name = "MessagePump", IsBackground = true };
            responderThread.Start();
        }

        private void Serialize(IMessage message, Stream outStream)
        {
            using (StreamWrapper wrapper = new StreamWrapper(outStream))
            {
                Envelope.Serialize(message, wrapper);
                //RaiseMessageSent(message, wrapper.GetOutputBytes());
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
                
                OnMessageReceived(
                    new MessageReceivedEventArgs(this, message, wrapper.GetInputBytes())
                );

                return message;
            }
        }

        protected void OnMessageReceived(MessageReceivedEventArgs args)
        {
            var evt = MessageReceived;
            if (evt != null)
            {
                evt(this, args);
            }
        }

        protected void OnClientClosed(ClientEventArgs args)
        {
            var evt = ClientClosed;
            if (evt != null)
            {
                evt(this, args);
            }
        }

        protected void MessagePump()
        {
            Exception currentException = null;

            try
            {
                running = true;

                while (running)
                {
                    ProcessMessages();
                    //ProcessAsynchronousMessages();
                }
            }
            catch (InvalidOperationException ex)
            {
                currentException = ex;
                LogClientError(ex, Level.Finer);
            }
            catch (IOException ex)
            {
                currentException = ex;
                LogClientError(ex, Level.Finer);
            }
            catch (Exception ex)
            {
                currentException = ex;
                LogClientError(ex, Level.Error);
            }
            finally
            {
                running = false;
                if (Transport.IsConnected)
                {
                    Transport.Disconnect(true);
                }
                OnClientClosed(new ClientEventArgs(this, currentException));
            }
        }


        private void ProcessMessages()
        {
            IMessage message = null;
            IMessage response = null;

            // read a line from socket
            using (var wrapper = new StreamWrapper(Transport.Stream))
            {
                message = Envelope.Deserialize(wrapper);
                message.Validate();

                MessageReceivedEventArgs args = new MessageReceivedEventArgs(this, message, wrapper.GetInputBytes());
                OnMessageReceived(args);

               // response = Dispatcher.DispatchMessage(message, this);
            }

        }

        private void LogClientError(Exception ex, Level level)
        {
            Logger.Logger.Log(null, level, "Message processing error for client '" + ConnectionID + "': " + ex.Message, ex);
            if (ex.InnerException != null)
                Logger.Logger.Log(null, level, "InnerException:\n" + ex.InnerException.Message, ex.InnerException);
        }

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

        public void Send(IMessage message)
        {
            //lock (sendLock)
            //{
                if (message == null)
                    throw new NullReferenceException("Attempt to send a null message");


                //Interlocked.Increment(ref ignoreResponses);
                Serialize(message, Transport.Stream);
                //messageSent.Set();
                Logger.InfoFormat("Sent message {0}", message.Name);
            //}
        }

        public T Send<T>(IMessage message) where T : IMessage
        {
            //lock (sendLock)
            {
                if (message == null)
                    throw new NullReferenceException("Attempt to send a null message");

                if (!typeof(T).IsAbstract && !typeof(T).IsInterface)
                    MessageFactory.Register(typeof(T));

                Serialize(message, Transport.Stream);
                //messageSent.Set();

                string endpoint = Transport.RemoteEndPoint == null ? "Unknown" : Transport.RemoteEndPoint.ToString();
                Logger.InfoFormat("Sent message {0} to {1}", message.Name, endpoint);

                // wait for a response
                //bool responded = sendResponded.WaitOne(MessageResponseTimeout, false);
                //if (!responded) throw new TimeoutException("Timeout waiting for message response");

                IMessage response = null;
                //sendResponse = null;

                //if (response is T)
                //    return (T)response;

                //// Let the protocol have a chance at handling an error response.  
                //string errorString = Protocol.IsError(response);
                //if (errorString != null)
                //	throw new ProtocolException(errorString);

                // If this is not an error message, we need to 
                // inform the caller that we did not receive what it wanted.
                string messageName = response != null ? response.Name : "(no message)";
                throw new Exception(string.Format("Unexpected Message {0} received in response to {1}", messageName, message.Name));
            }
        }

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
                StopMessagePump();
                //StopReceiveThread();

                if (Transport == null)
                    Logger.Error("Client transport is null in Disconnect");
                else
                {
                    Transport.Disconnect(forceDisconnect);
                    bool joined = responderThread.Join(5000);
                    // If we can't join the receive thread, disconnect the transport forcefully
                    if (!joined)
                        Transport.Disconnect(true);
                }

                OnClientClosed(null);
            }
        }

        private void StopMessagePump()
        {
            running = false;
        }

    }
}

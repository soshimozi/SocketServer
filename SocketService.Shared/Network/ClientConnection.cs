using System;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using SocketServer.Shared.Header;
using SocketServer.Shared;
using SocketServer.Shared.Sockets;
using System.Threading;
using SocketServer.Shared.Serialization;
using SocketServer.Shared.Messaging;
using System.Net.Sockets;
using System.Net;
using log4net;
using log4net.Core;

namespace SocketServer.Shared.Network
{
    public class ClientConnection //: Connection
    {
        private ServerAuthority _sa = null;

        private readonly object sendLock = new object();
        private readonly INetworkTransport client;

        private Thread responderThread;

        private volatile bool running = false;

        private static readonly ILog logger = LogManager.GetLogger(typeof(ClientConnection));

        public ClientConnection(MessageEnvelope envelope, INetworkTransport client)
        {
            this.client = client;
            Envelope = envelope;

            ClientId = Guid.NewGuid();
            _sa = new ServerAuthority(DHParameterHelper.GenerateParameters());
        }

        /// <summary>
        /// Connects this channel to the remote address and port.
        /// </summary>
        /// <param name="serverAddress">The server address to connect to.</param>
        /// <param name="port">The server port to connect to.</param>
        /// <returns>A Connected message providing information about the remote server.</returns>
        public void Connect(string serverAddress, int port)
        {
            Transport.Address = serverAddress;
            Transport.Port = port;
            Transport.Connect();

            StartReceiveThread();
        }

        public void Connect()
        {
            Connect(Transport.Address, Transport.Port);
        }

        public INetworkTransport Transport { get { return client;  } }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public Guid ClientId { get; set; }

        public ServerAuthority ServerAuthority
        {
            get { return _sa; }
        }

        public ClientBuffer ClientBuffer
        {
            get;
            set;
        }

        public RequestHeader RequestHeader
        {
            get;
            set;
        }

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
        public event EventHandler<DisconnectedArgs> ClientClosed;

        protected void OnMessageReceived(MessageEventArgs args)
        {
            
            var function = MessageReceived;
            if (function != null)
            {
                function(this, args);
            }
        }

        protected void OnMessageSent(MessageEventArgs args)
        {
            var function = MessageSent;
            if (function != null)
            {
                function(this, args);
            }
        }

        protected void OnClientClosed(DisconnectedArgs args)
        {
            var function = ClientClosed;
            if (function != null)
            {
                function(this, args);
            }
        }

        /// <summary>
        /// Starts a thread for Channels in Responder mode to process and handle incoming messages
        /// </summary>
        protected void StartReceiveThread()
        {
            if (running) return;

            responderThread = new Thread(ProcessMessages) { Name = "ClientConnectionThread", IsBackground = true };
            responderThread.Start();
        }

        protected void StopReceiveThread()
        {
            running = false;
        }

        private void ProcessMessages()
        {
            running = true;

            try
            {
                while (running)
                {
                    using (StreamWrapper wrapper = new StreamWrapper(client.Stream))
                    {
                        IMessage message = Envelope.Deserialize(wrapper) as IMessage;

                        MessageEventArgs args = new MessageEventArgs(this, message, wrapper.GetInputBytes());
                        OnMessageReceived(args);

                        //RaiseMessageReceived(message, wrapper.GetInputBytes());
                        //return message;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                client.Disconnect(true);

            }
        }

        public MessageEnvelope Envelope
        {
            get;
            set;
        }
        
        public static ClientConnection CreateClientConnection(MessageEnvelope envelope, INetworkTransport client)
        {
            ClientConnection connection = new ClientConnection(envelope, client);
            connection.StartReceiveThread();

            return connection;
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                return client.RemoteEndPoint;
            }
        }

        public void Send(IMessage message)
        {
            lock (sendLock)
            {
                if (message == null)
                    throw new NullReferenceException("Attempt to send a null message");

                using (StreamWrapper wrapper = new StreamWrapper(Transport.Stream))
                {
                    Envelope.Serialize(message, wrapper);
                }

                string endpoint = Transport.RemoteEndPoint == null ? "Unknown" : Transport.RemoteEndPoint.ToString();
                logger.Logger.Log(null, Level.Finer, string.Format("Sent message {0} to {1}", message.MessageID, endpoint), null);
                //Logger.w(Level.Finer, "Sent message {0} to {1}", message.MessageID, endpoint);
            }
        }

    }

}
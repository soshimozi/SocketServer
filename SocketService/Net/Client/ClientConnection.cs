using System;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using SocketServer.Shared.Header;
using SocketServer.Shared;
using SocketServer.Shared.Sockets;
using System.Threading;
using SocketServer.Shared.Serialization;
using System.Net.Sockets;
using System.Net;
using log4net;
using SocketServer.Shared.Messaging;
using System.ServiceModel;

namespace SocketServer.Net.Client
{
    public class ClientConnection //: Connection
    {
        private ServerAuthority _sa = null;
        private readonly INetworkTransport client;

        private Thread responderThread;

        private volatile bool running = false;

        private readonly object sendLock = new object();

        private static ILog Logger = LogManager.GetLogger(typeof(ClientConnection));

        //private readonly ZipSocket zipSocket;
        public ClientConnection(MessageEnvelope envelope, INetworkTransport client)
        {
            this.client = client;
            Envelope = envelope;
            //zipSocket = new ZipSocket(client);

            //ClientBuffer = buffer;
        }

        public INetworkTransport Transport { get { return client;  } }


        //public ZipSocket ClientSocket { get { return zipSocket; } }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the remote public key.
        /// </summary>
        /// <value>
        /// The remote public key.
        /// </value>
        //public AsymmetricKeyParameter RemotePublicKey { get; set; }

        //public DHParameters Parameters { get; set; }

        public ServerAuthority ServerAuthority
        {
            get { return _sa ?? (_sa = ServerAuthorityFactory.CreateServerAuthority()); }
            //set;
        }

        public ClientBuffer ClientBuffer
        {
            get;
            set;
        }

        public ProtocolState CurrentState
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

            while (running)
            {
                using (StreamWrapper wrapper = new StreamWrapper(client.Stream))
                {
                    IMessage message = Envelope.Deserialize(wrapper) as IMessage;
                    //RaiseMessageReceived(message, wrapper.GetInputBytes());
                    //return message;
                }
            }
        }

        public MessageEnvelope Envelope
        {
            get;
            private set;
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

        public T Send<T>(IMessage message) where T : IMessage
        {
            lock (sendLock)
            {
                if (message == null)
                    throw new NullReferenceException("Attempt to send a null message");

                //if (!typeof(T).IsAbstract && !typeof(T).IsInterface)
                //    MessageFactory.Register(typeof(T));

                using (StreamWrapper wrapper = new StreamWrapper(Transport.Stream))
                {
                    Envelope.Serialize(message, wrapper);
                }

                string endpoint = Transport.RemoteEndPoint == null ? "Unknown" : Transport.RemoteEndPoint.ToString();
                Logger.InfoFormat("Sent message {0} to {1}", message.Name, endpoint);

                // wait for a response
                //bool responded = sendResponded.WaitOne(MessageResponseTimeout, false);
                //if (!responded) throw new TimeoutException("Timeout waiting for message response");

                IMessage response = null;
                //sendResponse = null;

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

    }

}
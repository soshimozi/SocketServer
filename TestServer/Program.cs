using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using log4net;
using System.Collections;
using TestServer.Configuration;
using log4net.Config;
using log4net.Core;
using SocketServer.Shared.Network;
using SocketServer.Shared.Messaging;
using SocketServer.Shared.Reflection;

namespace TestServer
{
    class Program
    {
        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);
        private readonly object connectionListLock = new object();
        private readonly List<ClientConnection> connectionList = new List<ClientConnection>();

        private SocketListener listener = new SocketListener();
        
        private volatile bool running = false;

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Program instance = new Program();
            instance.Run(args);
        }

        private readonly Thread serverThread;
        public Program() 
        {
            // register messgaes
            RegisterMessages();

            serverThread = new Thread(new ThreadStart(ServerThread));
        }

        private void RegisterMessages()
        {
            var config 
                = ServerConfigurationHelper.GetServerConfiguration();

            if (config != null)
            {
                foreach (MessageConfigurationElement messageConfiguration in config.Messages)
                {

                    Type t = ReflectionHelper.FindType(messageConfiguration.TypeName);
                    if (t != null)
                    {
                       // MessageFactory.Register(messageConfiguration.Name, t);
                    }
                    else
                    {
                        throw new Exception("Invalid message type specified in configuration");
                    }
                }
            }
        }
    
        public void Run(string[] args)
        {
            if (!running)
            {
                serverThread.Start();

                while (true)
                {
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        Stop();
                        break;
                    }
                }
            }
        }

        public void Stop()
        {
            stopEvent.Set();
            if (!serverThread.Join(5000))
            {
                // it's hammer time
                serverThread.Abort();

                // log it
            }

            running = false;
        }

        protected void ServerThread()
        {
            listener.Initialize(4000);
            listener.Start();

            Logger.InfoFormat("Server started and listening on port {0}", 4000);

            running = true;

            while (!stopEvent.WaitOne(100))
            {
                INetworkTransport transport = listener.AcceptClient();
                if (transport != null)
                {
                    // let's wrap this with a client connection
                    ClientConnection connection = ClientConnection.CreateClientConnection(new PlainEnvelope(), transport);

                    // start pumping messages on this connection
                    //connection.StartMessagePump();

                    connection.MessageReceived += new EventHandler<MessageEventArgs>(connection_MessageReceived);

                    // add to client list
                    AddClient(connection);
                }
            }

            listener.Stop();
        }

        void connection_MessageReceived(object sender, MessageEventArgs e)
        {
        }

        protected void AddClient(ClientConnection connection)
        {
            lock (connectionListLock)
            {
                connectionList.Add(connection);
            }
        }

        //private List<Socket> BuildSocketList()
        //{
        //    var clientList = new List<Socket>();

        //    lock (connectionListLock)
        //    {
        //        try
        //        {
        //            clientList = connectionList.Select(c => c.Transport.Client).ToList();
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.ErrorFormat("Error: {0}", ex.Message);
        //        }
        //    }

        //    return clientList;
        //}
    }
}

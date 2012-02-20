using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using SocketServer.Shared.Serialization;
using SocketServer.Shared.Messaging;
using SocketServer.Shared.Network;
using SocketServer.Messages;
using SocketServer.Crypto;
using Org.BouncyCastle.Math;
using com.BlazeServer.Messages.MessageProtos;

namespace ClientConsole
{
    class Program
    {
        static int port = 4000;

        static bool encryptionEnabled = false;

        static byte[] serverPublicKey = null;
        static ServerAuthority sa = null;

        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            Console.Write("Connecting to ChannelListener...");

            ClientConnection client = new ClientConnection(new PlainEnvelope(), new SocketTransport());

            //TcpClient client = new TcpClient();
            //client.Connect("localhost", port);
            client.ClientClosed += new EventHandler<DisconnectedArgs>(client_ClientClosed);
            client.MessageReceived += new EventHandler<MessageEventArgs>(client_MessageReceived);

            client.Connect("localhost", port);

            Console.WriteLine("connected!");

            DateTime connectTime = DateTime.Now;
            Console.WriteLine(string.Format("Connected at {0}", connectTime));

            int count = 1;

            while (true)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                string command = "";

                while (true)
                {
                    command = Console.ReadLine();
                    headers.Clear();

                    while (true)
                    {
                        string line = Console.ReadLine();

                        // check for termination
                        if (line.Length == 0)
                            break;

                        string[] pieces = line.Split(':');
                        headers.Add(pieces[0].ToUpper(), pieces[1].ToUpper());
                    }

                    break;
                }

                MessageOne msg = new MessageOne();
                msg.MessageID = "MessageOne";
                msg.Value = 23;

                //scm.ControlMode = ControlMode.ChannelController;

                try
                {
                    var env = client.Envelope;
                    using (StreamWrapper wrapper = new StreamWrapper(client.Transport.Stream))
                    {
                        env.Serialize(msg, wrapper);
                        //((SocketTransport)client.Transport)
                        //IMessage response = client.Send<Message>(msg);
                        //Console.WriteLine(msg.Name);
                    }
                }
                catch (Exception pex)
                {
                    Console.WriteLine(pex.Message);
                }

                //CustomMessage message = new CustomMessage(command);
                //foreach (string key in headers.Keys)
                //    message.AddKeyValue(key, headers[key]);

                //Message response;
                //{
                //    try
                //    {
                //        response = client.SendAndReceive(message);
                //        Console.WriteLine("Message received: " + response.Name);
                //        Console.WriteLine(response.ToString());
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message + "\r\n");
                //        Console.WriteLine("Reconnecting...");
                //        Console.WriteLine(client.Connect());
                //        connectTime = DateTime.Now;
                //        Console.WriteLine(string.Format("Connected at {0}", connectTime));
                //    }
                //}

                TimeSpan ts = DateTime.Now - connectTime;
                Console.WriteLine(string.Format("  connection open for {0:D2}:{1:D2}:{2:D2} [{3} messages processed]\r\n", ts.Hours, ts.Minutes, ts.Seconds, count++));
            }
        }

        private static void client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message is ServerConnectionResponse)
            {
                HandleServerConnectResponse(e);
            }
            else if (e.Message is EnableEncryptionResponse)
            {
                HandleEnableEncryptionResponse(e);
            }

            //throw new NotImplementedException();
        }

        private static void HandleEnableEncryptionResponse(MessageEventArgs e)
        {
            EnableEncryptionResponse response = e.Message as EnableEncryptionResponse;
            encryptionEnabled = response.Success;

            if (encryptionEnabled)
            {
                e.ClientConnection.Envelope = new EncryptedMessageEnvelope((ICrypto)
                    new SharedKeyCrypto(
                        sa.Parameters,
                        sa.KeyPair,
                        serverPublicKey
                        )
                    );
            }
        }

        private static void HandleServerConnectResponse(MessageEventArgs e)
        {
            //ServerConnectionResponse response = e.Message as ServerConnectionResponse;

            //sa = new ServerAuthority(new BigInteger(response.DiffieHellmanInfo.P, 16), new BigInteger(response.DiffieHellmanInfo.G, 16));
            //serverPublicKey = response.DiffieHellmanInfo.PublicKeyInfo;

            //// now enable encryption
            //e.ClientConnection.Send(
            //    new EnableEncryptionMessage() 
            //    { 
            //        MessageID = "EnableEncryptionMessage", 
            //        Enable = true, 
            //        PublicKeyInfo = sa.GenerateEncodedPublicKeyInfo() 
            //    });
        }

        static void client_ClientClosed(object sender, DisconnectedArgs e)
        {
            //throw new NotImplementedException();
        }
    }

}

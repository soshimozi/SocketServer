using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using SocketService.Framework.Crypto;
using SocketService.Framework.Net.Sockets;
using SocketService.Framework.Response;
using SocketService.Framework.Request;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Serialize;

namespace ConsoleApplication1
{
    class Program
    {
        private DiffieHellmanKey _remotePublicKey;
        private DiffieHellmanProvider _provider;

        private Queue<ServerMessage> _inboundQueue = new Queue<ServerMessage>();
        private object _listLock = new object();

        private ZipSocket socket;

        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }

        public void Run(string[] args)
        {

            Console.WriteLine("Connecting to server...");
            ConnectToServer();

            Console.WriteLine("Negotiating keys...");
            NegotiateKeys();

            Console.WriteLine("Connected!");

            LoginResponse response;
            do
            {
                Console.WriteLine("Enter your user name: ");
                string userName = Console.ReadLine();

                // send login response
                LoginRequest request = new LoginRequest();
                request.LoginName = userName;

                SendObject(EncryptionType.AES, request);

                // Wait for a LoginResponse to come back
                response = WaitForObject<LoginResponse>(-1);

                if (response != null )
                {
                    Console.WriteLine(response.Message);
                }

            } while (response != null && !response.Success);

            if (response != null)
            {
                // start message pump thread
                Thread messageThread = new Thread(new ThreadStart(PumpMessages));
                messageThread.Start();

                bool quitFlag = false;

                DisplayMenu();
                do
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                        switch (keyinfo.Key)
                        {
                            case ConsoleKey.R:
                                Console.Write("Room Name: ");
                                string roomname = Console.ReadLine();
                                // change room
                                SendObject(EncryptionType.AES, new ChangeRoomRequest() { RoomName = roomname });

                                // there is no response for this request, other than a server message
                                // the request always succeeds, there are no private rooms
                                break;

                            case ConsoleKey.L:
                                SendObject(EncryptionType.AES, new ListUsersInRoomRequest());
                                ListUsersInRoomResponse listUsersResponse = WaitForObject<ListUsersInRoomResponse>(-1);

                                Console.WriteLine("Users:");
                                foreach (ServerUser user in listUsersResponse.Users)
                                {
                                    Console.WriteLine("{0}", user.Name);
                                }
                                Console.WriteLine();
                                break;

                            case ConsoleKey.X:
                                {
                                    Console.Write("Room: ");
                                    string room = Console.ReadLine();

                                    Console.WriteLine();
                                    Console.Write("Name: ");
                                    string name = Console.ReadLine();

                                    SendObject(EncryptionType.AES,
                                        new GetRoomVariableRequest() { RoomName = room, VariableName = name }
                                    );

                                    // Wait for a LoginResponse to come back
                                    GetRoomVariableResponse variableResponse = WaitForObject<GetRoomVariableResponse>(-1);
                                    if (variableResponse != null)
                                    {
                                        // display variable
                                        Console.WriteLine(variableResponse.ServerObject.GetValueForElement("__default__"));
                                    }

                                }
                                break;


                            case ConsoleKey.V:
                                {
                                    Console.Write("Room: ");
                                    string room = Console.ReadLine();

                                    Console.WriteLine();
                                    Console.Write("Name: ");
                                    string name = Console.ReadLine();

                                    Console.WriteLine();
                                    Console.Write("Value: ");
                                    string value = Console.ReadLine();

                                    ServerObject so = new ServerObject();
                                    so.SetElementValue("__default__", value, ServerObjectDataType.String);

                                    ServerObject[] soArray = new ServerObject[1];
                                    ServerObject arrayObject = new ServerObject();
                                    arrayObject.SetElementValue("value", 123, ServerObjectDataType.Integer);
                                    soArray[0] = arrayObject;

                                    so.SetElementValue("arrayTest", soArray, ServerObjectDataType.BzObjectArray);

                                    SendObject(EncryptionType.AES,
                                        new CreateRoomVariableRequest() { Room = room, VariableName = name, Value = so }
                                    );
                                    break;
                                }

                            case ConsoleKey.Q:
                                quitFlag = true;
                                break;
                        }

                        if( !quitFlag )
                            DisplayMenu();
                    }

                    if (!quitFlag)
                    {
                        // check if there are any messages available on the queue
                        // dispatch them

                        ServerMessage message = null;
                        lock (_listLock)
                        {
                            if (_inboundQueue.Count > 0)
                            {
                                message = _inboundQueue.Dequeue();
                            }
                        }
                        if (message != null)
                        {
                            DispatchMessage(message);
                        }
                    }


                } while (!_serverDisconnectedEvent.WaitOne(100) && !quitFlag);

                _stopEvent.Set();

            }

        }

        private T WaitForObject<T>(int milliseconds)  where T : class
        {
            IList readList = new List<Socket>() { socket.RawSocket };

            // now let's wait for messages
            Socket.Select(readList, null, null, milliseconds);

            T outp = default(T);

            // there is only one socket in the poll list
            // so if the count is greater than 0 then
            // the only one available should be the client socket
            if (readList.Count > 0)
            {
                if (socket.RawSocket.Available > 0)
                {
                    outp = ReadObject<T>(socket);
                }

            }

            return outp;
        }

        private void DispatchMessage(ServerMessage message)
        {
            Console.WriteLine("{0}", message.Message);
        }

        private void DisplayMenu()
        {
            Console.WriteLine("The following commands are available:");
            Console.WriteLine("\tR <room>\tChange Rooms (creates room if it doesn't exist)");
            Console.WriteLine("\tV <room> <name> <value>\tCreate a variable in the current room");
            Console.WriteLine("\tX <room> <name>\tRead variable from supplied room");
            Console.WriteLine("\tL\t\tList Users In Room");
            Console.WriteLine("\tQ\t\tQuit");
        }

        protected void PumpMessages()
        {
            // pump messages
            bool serverDown = false;

            ServerMessage message;
            while (!_stopEvent.WaitOne(50) && !(serverDown = PumpMessage(out message)))
            {
                if (!serverDown && message != null)
                {
                    // add message to outbound queue
                    lock (_listLock)
                    {
                        _inboundQueue.Enqueue(message);
                    }
                }
            }

            if (serverDown)
            {
                // set the disconnected event in the case the the server disconnected
                _serverDisconnectedEvent.Set();
            }
        }

        private void NegotiateKeys()
        {
            SendObject(EncryptionType.None, new GetCentralAuthorityRequest());
            bool doneHandshaking = false;
            int step = 0;

            // wait for central authority
            IList readList = new List<Socket>() { socket.RawSocket };
            while (!doneHandshaking)
            {
                Socket.Select(readList, null, null, -1);

                // there is only one socket in the poll list
                // so if the count is greater than 0 then
                // the only one available should be the client socket
                if (readList.Count > 0)
                {
                    int availableBytes = socket.RawSocket.Available;
                    if (availableBytes > 0)
                    {
                        //byte [] objectData = new byte[availableBytes];

                        byte[] objectData = socket.ReceiveData();
                        switch (step)
                        {
                            case 0:
                                CentralAuthority ca = ObjectSerialize.Deserialize<CentralAuthority>(objectData);
                                if (ca != null)
                                {
                                    _provider = ca.GetProvider();

                                    // Send Negotiate Key Command
                                    // Read Response when it comes back
                                    SendObject(EncryptionType.None,
                                        new NegotiateKeysRequest(_provider.PublicKey.ToByteArray()));

                                    step++;
                                }
                                break;

                            case 1:
                                // record server key
                                NegotiateKeysResponse response = ObjectSerialize.Deserialize<NegotiateKeysResponse>(objectData);
                                if (response != null)
                                {
                                    _remotePublicKey = _provider.Import(response.RemotePublicKey);
                                    doneHandshaking = true;
                                }
                                break;

                        }
                    }
                }
            }
        }

        private bool PumpMessage(out ServerMessage message)
        {
            bool serverDown = false;

            IList readList = new List<Socket>() { socket.RawSocket };

            // now let's wait for messages
            Socket.Select(readList, null, null, 0);

            message = null;

            // there is only one socket in the poll list
            // so if the count is greater than 0 then
            // the only one available should be the client socket
            if (readList.Count > 0)
            {
                // if socket is selected, and if available byes is 0, 
                // then socket has been closed
                serverDown = socket.RawSocket.Available == 0;

                if (!serverDown)
                {
                    message = ReadObject<ServerMessage>(socket);
                }

            }

            return serverDown;
        }

        private T ReadObject<T>(ZipSocket socket) where T : class
        {
            int availableBytes = socket.RawSocket.Available;
            if (availableBytes > 0)
            {
                byte[] objectData = socket.ReceiveData();

                // it should be a server message, we can look
                // at other message types later
                return ObjectSerialize.Deserialize<T>(objectData);
            }

            return null;
        }

        private void SendObject(EncryptionType encryptionType, object graph)
        {
            ClientRequestHeader header;
            if (encryptionType != EncryptionType.None)
            {
                using (Wrapper cryptoWrapper = Wrapper.CreateEncryptor(AlgorithmType.TripleDES, _provider.CreatePrivateKey(_remotePublicKey).ToByteArray()))
                {
                    byte[] requestData = ObjectSerialize.Serialize(graph);

                     header = new ClientRequestHeader(cryptoWrapper.IV,
                            EncryptionType.TripleDES, DateTime.Now, 0, cryptoWrapper.Encrypt(requestData));
                }
            }
            else
            {
                header = new ClientRequestHeader(new byte[0] { },
                        encryptionType, DateTime.Now, 0, ObjectSerialize.Serialize(graph));
            }

            SendData(socket, ObjectSerialize.Serialize(header));
        }

        private void SendData(ZipSocket client, byte[] data)
        {
            client.SendData(data);
        }


        private void ConnectToServer()
        {
            Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool connected = false;
            while (!connected)
            {
                try
                {
                    rawSocket.Connect("127.0.0.1", 4000);
                    connected = true;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("There was a network error.  Please make sure the server is started and listening.");
                    Thread.Sleep(5000);
                }
            }

            // wrap the socket
            socket = new ZipSocket(rawSocket, Guid.NewGuid());
        }
    }
}

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
using SocketService.Client.API;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Request;
using SocketService.Client.API.Event;
using SocketService.Client.API.Data;

namespace ConsoleApplication1
{
    class Program
    {
        ClientEngine _engine = new ClientEngine();
        Server _server = new Server();

        private long _loginState = 0;
        private AutoResetEvent _loginReceievedEvent = new AutoResetEvent(false);
        private AutoResetEvent _connectedReceivedEvent = new AutoResetEvent(false);

        private bool _connectionSuccessful = false;

        private string _userName = string.Empty;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }

        public void Run(string[] args)
        {

            _engine.ServerMessageRecieved += new EventHandler<ServerMessageReceivedArgs>(engine_ServerMessageRecieved);
            _engine.LoginResponseReceived += new EventHandler<LoginResponseEventArgs>(engine_LoginResponseReceived);
            _engine.GetRoomVariableResponseRecieved += new EventHandler<GetRoomVariableResponseArgs>(engine_GetRoomVariableResponseRecieved);
            _engine.JoinRoom += new EventHandler<JoinRoomEventArgs>(engine_JoinRoom);

            _server.ConnectionResponse += new EventHandler<ConnectionEventArgs>(server_ConnectionResponse);
            _engine.AddServer(_server);
            
            Console.WriteLine("Connecting to server...");

            while (!_connectionSuccessful)
            {
                _server.Connect("127.0.0.1", 4000);
                _connectedReceivedEvent.WaitOne(-1);

                if (!_connectionSuccessful)
                {
                    Console.WriteLine("Could not connect to server.  Retrying in 5 seconds.");
                    Thread.Sleep(5000);
                }
            }

            bool success = false;
            do
            {
                Console.Write("Enter your user name: ");
                _userName = Console.ReadLine();

                Console.WriteLine("Logging in...");
                Login(_userName);

                _loginReceievedEvent.WaitOne(-1);

                Interlocked.Read(ref _loginState);
                success = _loginState == 1;

                // wait until we got a login success
            } while (!success);


            bool quitFlag = false;

            DisplayMenu();
            do
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                switch (keyinfo.Key)
                {
                    case ConsoleKey.R:
                        Console.Write("Room Name: ");
                        JoinRoom(Console.ReadLine());
                        break;

                    //case ConsoleKey.L:
                    //    ListUsersInRoom();
                    //    break;

                    case ConsoleKey.X:
                        Console.Write("Room: ");
                        string room = Console.ReadLine();

                        Console.WriteLine();
                        Console.Write("Name: ");
                        string varname = Console.ReadLine();
                        GetRoomVariableRequest(room, varname);

                        break;


                    case ConsoleKey.V:
                        CreateRoomVariable();
                        break;

                    case ConsoleKey.Q:
                        quitFlag = true;
                        break;
                }

                if (!quitFlag)
                    DisplayMenu();

            } while (!quitFlag);

            // stop engine and disconnect all servers
            _engine.StopEngine();
        }

        void server_ConnectionResponse(object sender, ConnectionEventArgs e)
        {
            _connectionSuccessful = e.IsSuccessful;
            _connectedReceivedEvent.Set();
        }

        private void Login(string userName)
        {
            LoginRequest request = new LoginRequest();
            request.LoginName = userName;

            _server.SendRequestEncrypted(request);
        }


        private void GetRoomVariableRequest(string room, string varname)
        {
            //GetRoomVariableRequest grvr = new GetRoomVariableRequest();
            //grvr.RoomName = room;
            //grvr.VariableName = varname;

            //_server.SendRequestEncrypted(grvr);
        }

        private void JoinRoom(string roomName)
        {
            CreateRoomRequest crr = new CreateRoomRequest();

            crr.RoomName = roomName;
            _server.SendRequestEncrypted(crr);
        }

        void engine_JoinRoom(object sender, JoinRoomEventArgs e)
        {
            Room room = _engine.Managers.RoomManager.FindById(e.Event.RoomId);
            Console.WriteLine("You have entered {0}.", room.Name);
            Console.WriteLine("Users:");
            foreach (User user in room.Users)
            {
                Console.WriteLine("{1}{0}", user.UserName, user.IsMe ? "*" : "");
            }
        }

        private void CreateRoomVariable()
        {
            //Console.Write("Room: ");
            //string room = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Value: ");
            string stringValue = Console.ReadLine();

            SharedObject value = new SharedObject();
            value.SetElementValue("__default__", stringValue, SharedObjectDataType.String);

            SharedObject[] valueArray = new SharedObject[1];
            SharedObject arrayObject = new SharedObject();
            arrayObject.SetElementValue("value", 123, SharedObjectDataType.Integer);
            valueArray[0] = arrayObject;

            value.SetElementValue("arrayTest", valueArray, SharedObjectDataType.BzObjectArray);

            CreateRoomVariableRequest crvr = new CreateRoomVariableRequest();
            //crvr.RoomId = room;  get my room id
            crvr.Name = name;
            crvr.Value = value;

            _server.SendRequestEncrypted(crvr);
        }

        void engine_GetRoomVariableResponseRecieved(object sender, GetRoomVariableResponseArgs e)
        {
        }

        void engine_LoginResponseReceived(object sender, LoginResponseEventArgs e)
        {
            int state = e.LoginResponse.Success ? 1 : 0;
            Interlocked.Exchange(ref _loginState, state);

            _loginReceievedEvent.Set();
        }

        void engine_ServerMessageRecieved(object sender, ServerMessageReceivedArgs e)
        {
            DispatchMessage(e.Message);
        }

        private void DispatchMessage(string message)
        {
            Console.WriteLine("{0}", message);
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
    }
}

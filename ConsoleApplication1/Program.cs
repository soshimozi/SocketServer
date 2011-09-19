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

namespace ConsoleApplication1
{
    class Program
    {
        //ClientEngine _engine = new ClientEngine();
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
            _server.Engine.ServerMessageRecieved += new EventHandler<ServerMessageReceivedArgs>(engine_ServerMessageRecieved);
            _server.Engine.LoginResponseReceived += new EventHandler<LoginResponseEventArgs>(engine_LoginResponseReceived);
            _server.Engine.GetRoomVariableResponseRecieved += new EventHandler<GetRoomVariableResponseArgs>(engine_GetRoomVariableResponseRecieved);
            //_engine.ListUsersInRoomResponseReceived += new EventHandler<ListUsersInRoomResponseArgs>(engine_ListUsersInRoomResponseReceived);
            _server.Engine.JoinRoom += new EventHandler<JoinRoomEventArgs>(engine_JoinRoom);
            _server.Engine.ConnectionResponse += new EventHandler<ConnectionResponseEventArgs>(engine_ConnectionResponse);
            
            Console.WriteLine("Connecting to server...");

            while (!_connectionSuccessful)
            {
                _server.Engine.Connect();
                _connectedReceivedEvent.WaitOne(-1);

                if (!_connectionSuccessful)
                {
                    Console.WriteLine("Could not connect to server.  Retrying in 5 seconds.");
                    Thread.Sleep(5000);
                }
            }

            //while (!_engine.Connect())
            //{
            //    Console.WriteLine("Could not connect to server.  Retrying in 5 seconds.");
            //    Thread.Sleep(5000);
            //}

            //Console.WriteLine("Connected!");

            bool success = false;

            do
            {
                Console.Write("Enter your user name: ");
                _userName = Console.ReadLine();
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

            _server.Engine.StopEngine();
        }

        void engine_ConnectionResponse(object sender, ConnectionResponseEventArgs e)
        {
            _connectionSuccessful = e.IsSuccessful;
            _connectedReceivedEvent.Set();
        }

        private void Login(string userName)
        {
            LoginRequest request = new LoginRequest();
            request.LoginName = userName;

            _server.Engine.Send(request);
        }

        //private void ListUsersInRoom()
        //{
        //    ListUsersInRoomRequest request = new ListUsersInRoomRequest();
        //    _server.Engine.Send(request);
        //}

        private void GetRoomVariableRequest(string room, string varname)
        {
            GetRoomVariableRequest grvr = new GetRoomVariableRequest();
            grvr.RoomName = room;
            grvr.VariableName = varname;

            _server.Engine.Send(grvr);
        }

        private void JoinRoom(string roomName)
        {
            CreateRoomRequest crr = new CreateRoomRequest();

            crr.RoomName = roomName;
            _server.Engine.Send(crr);
        }

        void engine_JoinRoom(object sender, JoinRoomEventArgs e)
        {
            Room room = _server.Managers.RoomManager.FindById(e.Event.RoomId);

            //if (e.Event.e == _userName)
            //{
            //    Console.WriteLine("You have entered {0}.", e.RoomName);
            //}
            //else
            //{
            //    Console.WriteLine("{0} has entered the room.", e.UserName);
            //}
        }

        private void CreateRoomVariable()
        {
            Console.Write("Room: ");
            string room = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Value: ");
            string stringValue = Console.ReadLine();

            ServerObject value = new ServerObject();
            value.SetElementValue("__default__", stringValue, ServerObjectDataType.String);

            ServerObject[] valueArray = new ServerObject[1];
            ServerObject arrayObject = new ServerObject();
            arrayObject.SetElementValue("value", 123, ServerObjectDataType.Integer);
            valueArray[0] = arrayObject;

            value.SetElementValue("arrayTest", valueArray, ServerObjectDataType.BzObjectArray);

            CreateRoomVariableRequest crvr = new CreateRoomVariableRequest();
            crvr.Room = room;
            crvr.VariableName = name;
            crvr.Value = new RoomVariable() { RoomId = 0, Value = value };

            _server.Engine.Send(crvr);
        }

        //void engine_ListUsersInRoomResponseReceived(object sender, ListUsersInRoomResponseArgs e)
        //{
        //    Console.WriteLine("Users:");
        //    foreach (ServerUser user in e.Response.Users)
        //    {
        //        Console.WriteLine("{0}", user.Name);
        //    }
        //    Console.WriteLine();
        //}

        void engine_GetRoomVariableResponseRecieved(object sender, GetRoomVariableResponseArgs e)
        {
            Console.Write(e.Response.Room);
            Console.Write(" : ");
            Console.WriteLine(e.Response.RoomVariable.Value .GetValueForElement("__default__"));
        }

        void engine_LoginResponseReceived(object sender, LoginResponseEventArgs e)
        {
            //DispatchMessage(e.LoginResponse.UserName);
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

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

namespace ConsoleApplication1
{
    class Program
    {
        ClientEngine _engine = new ClientEngine();

        private long _loginState = 0;
        private AutoResetEvent _loginReceievedEvent = new AutoResetEvent(false);

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
            _engine.ListUsersInRoomResponseReceived += new EventHandler<ListUsersInRoomResponseArgs>(engine_ListUsersInRoomResponseReceived);
            
            Console.WriteLine("Connecting to server...");

            while (!_engine.Connect())
            {
                Console.WriteLine("Could not connect to server.  Retrying in 5 seconds.");
                Thread.Sleep(5000);
            }

            Console.WriteLine("Connected!");

            bool success = false;

            do
            {
                Console.Write("Enter your user name: ");
                string loginName = Console.ReadLine();
                _engine.Login(loginName);

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
                        string roomname = Console.ReadLine();
                        _engine.ChangeRoom(roomname);
                        break;

                    case ConsoleKey.L:
                        _engine.ListUsersInRoom();
                        break;

                    case ConsoleKey.X:
                        Console.Write("Room: ");
                        string room = Console.ReadLine();

                        Console.WriteLine();
                        Console.Write("Name: ");
                        string varname = Console.ReadLine();
                        _engine.GetRoomVariable(room, varname);

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

            _engine.StopEngine();

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

            _engine.CreateRoomVariable(room, name, value);
        }

        void engine_ListUsersInRoomResponseReceived(object sender, ListUsersInRoomResponseArgs e)
        {
            Console.WriteLine("Users:");
            foreach (ServerUser user in e.Response.Users)
            {
                Console.WriteLine("{0}", user.Name);
            }
            Console.WriteLine();
        }

        void engine_GetRoomVariableResponseRecieved(object sender, GetRoomVariableResponseArgs e)
        {
            Console.Write(e.Response.Name);
            Console.Write(" : ");
            Console.WriteLine(e.Response.Value.GetValueForElement("__default__"));
        }

        void engine_LoginResponseReceived(object sender, LoginResponseEventArgs e)
        {
            DispatchMessage(e.LoginResponse.Message);
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

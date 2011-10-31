using System;
using System.Threading;
using SocketService.Client.API;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Client.SharedObjects;
using SocketService.Client.API.Event;

namespace ConsoleApplication1
{
    class Program
    {
        readonly ClientEngine _engine = new ClientEngine();
        readonly Server _server = new Server();

        private long _loginState;
        private readonly AutoResetEvent _loginReceievedEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _connectedReceivedEvent = new AutoResetEvent(false);

        private bool _connectionSuccessful;

        private string _userName = string.Empty;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run(args);
        }

        public void Run(string[] args)
        {

            _engine.ServerMessageRecieved += 
                (o, receivedArgs) => {
                    if (receivedArgs != null)
                        EngineServerMessageRecieved(receivedArgs);
                };

            _engine.LoginResponseReceived += 
                (o, eventArgs) => {
                    if (eventArgs != null)
                        EngineLoginResponseReceived(eventArgs);
                };

            _engine.GetRoomVariableResponseRecieved +=
                (o, responseArgs) => {
                    if (responseArgs != null)
                        engine_GetRoomVariableResponseRecieved(responseArgs);
                };

            _engine.JoinRoom += (o, eventArgs) => { if (eventArgs != null) EngineJoinRoom(eventArgs); };

            _server.ConnectionResponse += 
                (o, eventArgs) => {
                    if (eventArgs != null)
                        ServerConnectionResponse(eventArgs);
                };

            _engine.AddServer(_server);
            
            Console.WriteLine("Connecting to server...");

            while (!_connectionSuccessful)
            {
                _server.Connect("127.0.0.1", 4000);
                _connectedReceivedEvent.WaitOne(-1);

                if (_connectionSuccessful) continue;
                Console.WriteLine("Could not connect to server.  Retrying in 5 seconds.");
                Thread.Sleep(5000);
            }

            bool success;
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

        void ServerConnectionResponse(ConnectionEventArgs e = null)
        {
            if (e == null) throw new ArgumentNullException("e");
            _connectionSuccessful = e.IsSuccessful;
            _connectedReceivedEvent.Set();
        }

        private void Login(string userName)
        {
            var request = new LoginRequest {LoginName = userName};

            _server.SendRequestEncrypted(request);
        }


        private void GetRoomVariableRequest(string room, string varname)
        {
            if( string.IsNullOrEmpty(room) || string.IsNullOrEmpty(varname))
            {
                
            }

            //GetRoomVariableRequest grvr = new GetRoomVariableRequest();
            //grvr.RoomName = room;
            //grvr.VariableName = varname;

            //_server.SendRequestEncrypted(grvr);
        }

        private void JoinRoom(string roomName)
        {
            var crr = new CreateRoomRequest {RoomName = roomName};

            _server.SendRequestEncrypted(crr);
        }

        void EngineJoinRoom(JoinRoomEventArgs e = null)
        {
            if (e == null) throw new ArgumentNullException("e");

            var room = _engine.Managers.RoomManager.FindById(e.Event.RoomId);
            Console.WriteLine("You have entered {0}.", room.Name);
            Console.WriteLine("Users:");
            foreach (var user in room.Users)
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

            var value = new SharedObject();
            value.SetElementValue("__default__", stringValue, SharedObjectDataType.String);

            var valueArray = new SharedObject[1];
            var arrayObject = new SharedObject();
            arrayObject.SetElementValue("value", 123, SharedObjectDataType.Integer);
            valueArray[0] = arrayObject;

            value.SetElementValue("arrayTest", valueArray, SharedObjectDataType.BzObjectArray);

            long roomId = _engine.Managers.UserManager.Me.Room.RoomId;
            var crvr = new CreateRoomVariableRequest {Name = name, Value = value, RoomId = roomId };
            //crvr.RoomId = room;  get my room id

            _server.SendRequestEncrypted(crvr);
        }

        void engine_GetRoomVariableResponseRecieved(GetRoomVariableResponseArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");
        }

        void EngineLoginResponseReceived(LoginResponseEventArgs e = null)
        {
            if (e == null) throw new ArgumentNullException("e");
            var state = e.LoginResponse.Success ? 1 : 0;
            Interlocked.Exchange(ref _loginState, state);

            _loginReceievedEvent.Set();
        }

        void EngineServerMessageRecieved(ServerMessageReceivedArgs e = null)
        {
            if (e == null) throw new ArgumentNullException("e");
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

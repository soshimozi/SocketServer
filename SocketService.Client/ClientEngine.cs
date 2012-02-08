using System;
using System.Collections.Generic;
using SocketServer.Client.Manager;
using SocketServer.Client;
using SocketServer.Shared.Response;

namespace SocketServer.Client
{
    public class ClientEngine
    {
        public event EventHandler<ServerMessageReceivedArgs> ServerMessageRecieved;
        public event EventHandler<LoginResponseEventArgs> LoginResponseReceived;
        public event EventHandler<GetRoomVariableResponseArgs> GetRoomVariableResponseRecieved;
        public event EventHandler<JoinRoomEventArgs> JoinRoom;
        public event EventHandler<RoomUserUpdateEventArgs> RoomUserUpdate;
        public event EventHandler<LeaveRoomEventArgs> LeaveRoom;
        public event EventHandler<RoomVariableUpdateArgs> RoomVariableUpdate;

        private readonly List<Server> _servers = new List<Server>();

        public ClientEngine()
        {
            Managers = new ManagerHelper(this);
        }

        public ManagerHelper Managers { get; private set; }

        public void AddServer(Server server)
        {
            lock (_servers)
            {
                _servers.Add(server);
            }

            server.ServerEvent += ServerEvent;
            server.ServerResponse += ServerResponse;
        }

        public void StopEngine()
        {
            lock (_servers)
            {
                foreach (var server in _servers)
                {
                    server.Disconnect();
                }
            }
        }

        protected void ServerResponse(object sender, ServerResponseEventArgs e)
        {
            if (e.Response is GetRoomVariableResponse)
            {
                OnGetRoomVariableResponseRecieved(
                    new GetRoomVariableResponseArgs
                    {
                        Response = e.Response as GetRoomVariableResponse
                    }
                    );
            }
            else if (e.Response is LoginResponse)
            {
                OnLoginResponseReceieved(
                    new LoginResponseEventArgs
                    {
                        Response = e.Response as LoginResponse
                    }
                    );
            }

        }

        protected void ServerEvent(object sender, ServerEventEventArgs e)
        {
            if (e.ServerEvent is JoinRoomEvent)
            {
                OnJoinRoomEvent(
                    new JoinRoomEventArgs
                    {
                        Event = e.ServerEvent as JoinRoomEvent
                    }
                    );
            }
            else if (e.ServerEvent is RoomUserUpdateEvent)
            {
                OnRoomUserUpdate(
                    new RoomUserUpdateEventArgs
                    {
                        Event = e.ServerEvent as RoomUserUpdateEvent
                    }
                    );
            }
            else if (e.ServerEvent is RoomVariableUpdateEvent)
            {
                OnRoomVariableUpdate(
                    new RoomVariableUpdateArgs
                    {
                        Event = e.ServerEvent as RoomVariableUpdateEvent
                    }
                    );
            }
        }

        protected virtual void OnRoomVariableUpdate(RoomVariableUpdateArgs args)
        {
            var func = RoomVariableUpdate;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnLeaveRoom(LeaveRoomEventArgs args)
        {
            var func = LeaveRoom;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnRoomUserUpdate(RoomUserUpdateEventArgs args)
        {
            var func = RoomUserUpdate;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnGetRoomVariableResponseRecieved(GetRoomVariableResponseArgs args)
        {
            var func = GetRoomVariableResponseRecieved;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnServerMessageReceived(ServerMessageReceivedArgs args)
        {
            var func = ServerMessageRecieved;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnLoginResponseReceieved(LoginResponseEventArgs args)
        {
            var func = LoginResponseReceived;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnJoinRoomEvent(JoinRoomEventArgs args)
        {
            var func = JoinRoom;
            if (func != null)
            {
                func(this, args);
            }
        }
    }
}
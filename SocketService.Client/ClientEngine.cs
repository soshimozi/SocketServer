using System;
using System.Collections.Generic;
using SocketService.Client.Manager;
using SocketService.Event;
using SocketService.Shared.Response;

namespace SocketService.Client
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

            server.ServerEvent += ServerServerEvent;
            server.ServerResponse += ServerServerResponse;
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

        protected void ServerServerResponse(object sender, ServerResponseEventArgs e)
        {
            HandleServerResponse(e.Response);
        }

        protected void ServerServerEvent(object sender, ServerEventEventArgs e)
        {
            HandleEvent(e.ServerEvent);
        }

        private void HandleServerResponse(IServerResponse response)
        {
            if (response is GetRoomVariableResponse)
            {
                OnGetRoomVariableResponseRecieved(
                    new GetRoomVariableResponseArgs
                        {
                            Response = response as GetRoomVariableResponse
                        }
                    );
            }
            else if (response is LoginResponse)
            {
                OnLoginResponseReceieved(
                    new LoginResponseEventArgs
                        {
                            Response = response as LoginResponse
                        }
                    );
            }
        }

        private void HandleEvent(IEvent evt)
        {
            if (evt is JoinRoomEvent)
            {
                OnJoinRoomEvent(
                    new JoinRoomEventArgs
                        {
                            Event = evt as JoinRoomEvent
                        }
                    );
            }
            else if (evt is RoomUserUpdateEvent)
            {
                OnRoomUserUpdate(
                    new RoomUserUpdateEventArgs
                        {
                            Event = evt as RoomUserUpdateEvent
                        }
                    );
            }
            else if (evt is RoomVariableUpdateEvent)
            {
                OnRoomVariableUpdate(
                    new RoomVariableUpdateArgs
                        {
                            Event = evt as RoomVariableUpdateEvent
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
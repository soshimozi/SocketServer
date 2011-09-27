using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Crypto;
using SocketService.Framework.Client.Sockets;
using SocketService.Framework.Client.Response;
using System.Net.Sockets;
using System.Threading;
using SocketService.Framework.Client.Serialize;
using System.Collections;
using SocketService.Framework.Request;
using SocketService.Framework.SharedObjects;
using SocketService.Client.API.Command;
using SocketService.Framework.Client.Event;
using SocketService.Client.API.Event;
using SocketService.Client.API.Manager;

namespace SocketService.Client.API
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

        public ManagerHelper Managers
        {
            get;
            private set;
        }

        public void AddServer(Server server)
        {
            lock (_servers)
            { _servers.Add(server); }

            server.ServerEvent += new EventHandler<ServerEventEventArgs>(server_ServerEvent);
            server.ServerResponse += new EventHandler<ServerResponseEventArgs>(server_ServerResponse);

        }

        public void StopEngine()
        {
            lock (_servers)
            {
                foreach (Server server in _servers)
                {
                    server.Disconnect();
                }
            }
        }

        protected void server_ServerResponse(object sender, ServerResponseEventArgs e)
        {
            HandleServerResponse(e.Response);
        }

        protected void server_ServerEvent(object sender, ServerEventEventArgs e)
        {
            HandleEvent(e.ServerEvent);
        }

        private void HandleServerResponse(IResponse response)
        {
            if( response is GetRoomVariableResponse)
            {
                OnGetRoomVariableResponseRecieved(
                    new GetRoomVariableResponseArgs()
                    {
                        Response = response as GetRoomVariableResponse
                    }
                );
            }
            else if (response is LoginResponse)
            {
                OnLoginResponseReceieved(
                    new LoginResponseEventArgs()
                    {
                        LoginResponse = response as LoginResponse
                    }
                );
            }
        }

        private void HandleEvent(IEvent evt)
        {
            if (evt is JoinRoomEvent)
            {
                OnJoinRoomEvent(
                    new JoinRoomEventArgs()
                    {
                        Event = evt as JoinRoomEvent
                    }
                );
            }
            else if (evt is RoomUserUpdateEvent)
            {
                OnRoomUserUpdate(
                    new RoomUserUpdateEventArgs()
                    {
                        Event = evt as RoomUserUpdateEvent
                    }
               );
            }
            else if (evt is RoomVariableUpdateEvent)
            {
                OnRoomVariableUpdate(
                    new RoomVariableUpdateArgs()
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

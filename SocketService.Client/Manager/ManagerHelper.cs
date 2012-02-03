using System;
using SocketServer.Client.Data;
using SocketServer.Event;

namespace SocketServer.Client.Manager
{
    public class ManagerHelper
    {
        private readonly ClientEngine _engine;
        public ManagerHelper(ClientEngine engine)
        {
            if (engine == null)
                throw new ArgumentNullException();

            RoomManager = new RoomManager();
            UserManager = new UserManager();

            _engine = engine;

            _engine.RoomUserUpdate += EngineRoomUserUpdate;
            _engine.JoinRoom += EngineJoinRoom;
            _engine.LoginResponseReceived += EngineLoginResponseReceived;
            _engine.RoomVariableUpdate += EngineRoomVariableUpdate;
            _engine.LeaveRoom += EngineLeaveRoom;
        }

        void EngineLeaveRoom(object sender, LeaveRoomEventArgs e)
        {
            RoomManager.RemoveRoom(e.RoomId);
        }

        void EngineRoomVariableUpdate(object sender, RoomVariableUpdateArgs e)
        {
            switch (e.Event.Action)
            {
                case RoomVariableUpdateAction.Add:
                    RoomManager.AddRoomVariable(e.Event.RoomId, e.Event.Name, e.Event.Value);
                    break;

                case RoomVariableUpdateAction.Delete:
                    RoomManager.DeleteRoomVariable(e.Event.RoomId, e.Event.Name);
                    break;

                case RoomVariableUpdateAction.Update:
                    RoomManager.UpdateRoomVariable(e.Event.RoomId, e.Event.Name, e.Event.Value);
                    break;
            }
        }
    
        void EngineLoginResponseReceived(object sender, LoginResponseEventArgs e)
        {

            if (!e.Response.Success) return;

            var user = new ClientUser {IsMe = true, UserName = e.Response.UserName};

            //foreach (string current in loginResponse.UserVariables.Keys)
            //{
            //    EsObject value = loginResponse.UserVariables[current] as EsObject;
            //    user.AddUserVariable(new UserVariable
            //    {
            //        Name = current,
            //        Value = value
            //    });
            //}
            UserManager.AddUser(user);
            UserManager.Me = user;
        }

        void EngineJoinRoom(object sender, JoinRoomEventArgs e)
        {
            var joinRoomEvent = e.Event;
            if (RoomManager == null) return;

            var room = RoomManager.FindById(joinRoomEvent.RoomId);
            if (room == null)
            {
                room = new ClientRoom(joinRoomEvent.RoomId) { Name = joinRoomEvent.RoomName };
                RoomManager.AddRoom(room);
            }

            room.Description = joinRoomEvent.RoomDescription;
            room.IsProtected = joinRoomEvent.Protected;
            room.Capacity = joinRoomEvent.Capacity;
            room.IsHidden = joinRoomEvent.Hidden;

            //foreach (SharedObject roomVariable in joinRoomEvent.RoomVariables)
            //{
            //    room.AddRoomVariable(roomVariable.Name, roomVariable);
            //}

            foreach (var userListEntry in joinRoomEvent.Users)
            {
                var u = UserManager.AddUser(UserListEntryToUser(userListEntry));
                room.AddUser(u);
            }

            if (UserManager != null) UserManager.Me.Room = room;
        }

        private ClientUser UserListEntryToUser(string userName)
        {
            return new ClientUser
            {
                UserName = userName,
                IsMe = false
            };
        }

        void EngineRoomUserUpdate(object sender, RoomUserUpdateEventArgs e)
        {
            var roomUserUpdateEvent = e.Event;

            var user = UserManager.FindByName(roomUserUpdateEvent.UserName);
            var room = RoomManager.FindById(roomUserUpdateEvent.RoomId);
            if (user == null)
            {
                user = new ClientUser { UserName = roomUserUpdateEvent.UserName };
                //foreach (UserVariable current in roomUserUpdateEvent.UserVariables)
                //{
                //    user.AddUserVariable(current);
                //}
            }

            switch (roomUserUpdateEvent.Action)
            {
                case RoomUserUpdateAction.AddUser:
                        user = UserManager.AddUser(user);
                        room.AddUser(user);
                        break;
                case RoomUserUpdateAction.DeleteUser:
                        UserManager.RemoveUser(user.UserName);
                        room.RemoveUser(user);
                        break;
            }
        }

        public RoomManager RoomManager
        {
            get;
            private set;
        }

        public UserManager UserManager
        {
            get;
            private set;
        }

    }
}

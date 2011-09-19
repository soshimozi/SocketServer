using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Client.Event;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Client.Response;

namespace SocketService.Client.API
{
    public class ManagerHelper
    {
        private readonly ClientEngine _engine;
        public ManagerHelper(ClientEngine engine)
        {
            _engine = engine;

            this.RoomManager = new RoomManager();
            this.UserManager = new UserManager();

            engine.RoomUserUpdate += new EventHandler<RoomUserUpdateEventArgs>(engine_RoomUserUpdate);
            engine.JoinRoom += new EventHandler<JoinRoomEventArgs>(engine_JoinRoom);
            engine.LoginResponseReceived += new EventHandler<LoginResponseEventArgs>(engine_LoginResponseReceived);
        }

        void engine_LoginResponseReceived(object sender, LoginResponseEventArgs e)
        {
            LoginResponse loginResponse = e.LoginResponse;
            if (loginResponse.Success)
            {
                User user = new User();
                user.IsMe = true;
                user.UserName = loginResponse.UserName;

                //foreach (string current in loginResponse.UserVariables.Keys)
                //{
                //    EsObject value = loginResponse.UserVariables[current] as EsObject;
                //    user.AddUserVariable(new UserVariable
                //    {
                //        Name = current,
                //        Value = value
                //    });
                //}
                this.UserManager.AddUser(user);
                this.UserManager.Me = user;
            }
        }

        void engine_JoinRoom(object sender, JoinRoomEventArgs e)
        {
            JoinRoomEvent joinRoomEvent = e.Event;
            Room room = this.RoomManager.FindById(joinRoomEvent.RoomId);
            if (room == null)
            {
                room = new Room();
                room.Name = joinRoomEvent.RoomName;
                RoomManager.AddRoom(room);
            }

            room.Description = joinRoomEvent.RoomDescription;
            room.IsProtected = joinRoomEvent.Protected;
            room.Capacity = joinRoomEvent.Capacity;
            room.IsHidden = joinRoomEvent.Hidden;

            foreach (RoomVariable roomVariable in joinRoomEvent.RoomVariables)
            {
                room.AddRoomVariable(roomVariable);
            }

            foreach (UserListEntry userListEntry in joinRoomEvent.Users)
            {
                User u = this.UserManager.AddUser(this.UserListEntryToUser(userListEntry));
                room.AddUser(u);
            }
        }

        private User UserListEntryToUser(UserListEntry entry)
        {
            return new User
            {
                UserName = entry.UserName,
                IsMe = false
            };
        }

        void engine_RoomUserUpdate(object sender, RoomUserUpdateEventArgs e)
        {
            RoomUserUpdateEvent roomUserUpdateEvent = e.Event;

            User user = this.UserManager.FindByName(roomUserUpdateEvent.UserName);
            Room room = this.RoomManager.FindById(roomUserUpdateEvent.RoomId);
            if (user == null)
            {
                user = new User();
                user.UserName = roomUserUpdateEvent.UserName;
                //foreach (UserVariable current in roomUserUpdateEvent.UserVariables)
                //{
                //    user.AddUserVariable(current);
                //}
            }
            switch (roomUserUpdateEvent.Action)
            {
                case RoomUserUpdateAction.AddUser:
                        user = this.UserManager.AddUser(user);
                        room.AddUser(user);
                        break;
                case RoomUserUpdateAction.DeleteUser:
                        this.UserManager.RemoveUser(user.UserName);
                        room.RemoveUser(user.UserName);
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

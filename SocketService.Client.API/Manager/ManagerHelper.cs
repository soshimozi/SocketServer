using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Client.Event;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Client.Response;
using SocketService.Client.API.Event;
using SocketService.Client.API.Data;

namespace SocketService.Client.API.Manager
{
    public class ManagerHelper
    {
        private ClientEngine _engine;
        public ManagerHelper(/*ClientEngine engine*/)
        {
            //_engine = engine;

            this.RoomManager = new RoomManager();
            this.UserManager = new UserManager();

            //engine.RoomUserUpdate += new EventHandler<RoomUserUpdateEventArgs>(engine_RoomUserUpdate);
            //engine.JoinRoom += new EventHandler<JoinRoomEventArgs>(engine_JoinRoom);
            //engine.LoginResponseReceived += new EventHandler<LoginResponseEventArgs>(engine_LoginResponseReceived);
        }

        public ClientEngine ClientEngine
        {
            set
            {
                _engine = value;

                _engine.RoomUserUpdate += new EventHandler<RoomUserUpdateEventArgs>(engine_RoomUserUpdate);
                _engine.JoinRoom += new EventHandler<JoinRoomEventArgs>(engine_JoinRoom);
                _engine.LoginResponseReceived += new EventHandler<LoginResponseEventArgs>(engine_LoginResponseReceived);
                _engine.RoomVariableUpdate += new EventHandler<RoomVariableUpdateArgs>(engine_RoomVariableUpdate);
                _engine.LeaveRoom += new EventHandler<LeaveRoomEventArgs>(engine_LeaveRoom);
            }

        }

        void engine_LeaveRoom(object sender, LeaveRoomEventArgs e)
        {
            this.RoomManager.RemoveRoom(e.RoomId);
        }

        void engine_RoomVariableUpdate(object sender, RoomVariableUpdateArgs e)
        {
            //switch(e.Event.Action)
            //{
            //    case RoomVariableUpdateAction.Add:
            //        RoomManager.AddRoomVariable(e.Event.RoomId, e.Event.Name, e.Event.Variable);
            //        break;

            //    case RoomVariableUpdateAction.Delete:
            //        RoomManager.DeleteRoomVariable(e.Event.RoomId, e.Event.Name);
            //        break;

            //    case RoomVariableUpdateAction.Update:
            //        RoomManager.UpdateRoomVariable(e.Event.RoomId, e.Event.Name, e.Event.Variable);
            //        break;
            //}
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

            //foreach (SharedObject roomVariable in joinRoomEvent.RoomVariables)
            //{
            //    room.AddRoomVariable(roomVariable.Name, roomVariable);
            //}

            foreach (UserListEntry userListEntry in joinRoomEvent.Users)
            {
                User u = this.UserManager.AddUser(this.UserListEntryToUser(userListEntry));
                room.AddUser(u);
            }

            this.UserManager.Me.Room = room;
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

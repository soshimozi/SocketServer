using System;

namespace SocketServer.Client
{
    public class RoomUserUpdateEventArgs : EventArgs
    {
        public RoomUserUpdateEvent Event
        {
            get;
            set;
        }

        //public int RoomId
        //{
        //    get;
        //    set;
        //}

        //public string UserName
        //{
        //    get;
        //    set;
        //}

        //public RoomUserUpdateAction Action
        //{
        //    get;
        //    set;
        //}
    }
}

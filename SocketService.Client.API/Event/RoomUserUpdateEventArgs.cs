using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API.Event
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

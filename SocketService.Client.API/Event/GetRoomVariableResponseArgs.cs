using System;
using SocketService.Framework.Client.Response;

namespace SocketService.Client.API.Event
{
    public class GetRoomVariableResponseArgs : EventArgs
    {
        public GetRoomVariableResponse Response
        {
            get;
            set;
        }
    }
}

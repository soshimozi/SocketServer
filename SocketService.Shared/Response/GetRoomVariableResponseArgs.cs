using System;

namespace SocketServer.Shared.Response
{
    public class GetRoomVariableResponseArgs : EventArgs
    {
        public GetRoomVariableResponse Response { get; set; }
    }
}

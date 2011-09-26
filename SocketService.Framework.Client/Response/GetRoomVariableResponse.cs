using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class GetRoomVariableResponse : IResponse
    {
        public int ZoneId
        {
            get;
            set;
        }

        public int RoomId
        {
            get;
            set;
        }
    
        public string Name
        {
            get;
            set;
        }
    
        public SharedObject Value
        {
            get;
            set;
        }
    }
}

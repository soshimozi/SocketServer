using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class DeleteRoomVariableRequest 
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
    }
}

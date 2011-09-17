using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class GetRoomVariableRequest
    {
        public string RoomName
        {
            get;
            set;
        }

        public string VariableName
        {
            get;
            set;
        }
    }
}

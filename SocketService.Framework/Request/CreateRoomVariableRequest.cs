using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class CreateRoomVariableRequest
    {
        public string Room
        {
            get;
            set;
        }

        public string VariableName
        {
            get;
            set;
        }

        public ServerObject Value
        {
            get;
            set;
        }
    }
}

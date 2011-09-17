using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Response
{
    [Serializable]
    public class GetRoomVariableResponse
    {
        public ServerObject ServerObject
        {
            get;
            set;
        }
    }
}

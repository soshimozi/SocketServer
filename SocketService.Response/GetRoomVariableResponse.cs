using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Response
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class DeleteRoomVariableResponse : IResponse
    {
        public string Room
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

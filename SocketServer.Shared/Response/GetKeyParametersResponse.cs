using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Shared.Response
{
    [Serializable]
    public class GetKeyParametersResponse
    {
        public byte[] G
        {
            get;
            set;
        }

        public byte[] P
        {
            get;
            set;
        }
    }
}

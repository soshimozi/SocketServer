using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Crypto;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class GetCentralAuthorityRequest 
    {
        public CentralAuthority CentralAuthority
        {
            get;
            set;
        }
    }
}

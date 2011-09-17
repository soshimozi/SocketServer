using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Response
{
    [Serializable]
    public class LoginResponse
    {
        public bool Success
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}

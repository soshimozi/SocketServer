using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.API
{
    public class UserPublicMessageContext
    {
        public string Zone
        {
            get;
            set;
        }

        public string User
        {
            get;
            set;
        }

        public string Room
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

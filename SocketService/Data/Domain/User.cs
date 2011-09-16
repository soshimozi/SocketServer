using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Data.Domain
{
    public class User
    {
        public string UserName
        {
            get;
            set;
        }

        public string Room
        {
            get;
            set;
        }

        public Guid ClientKey
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Client.API
{
    public class Server
    {
        public Server()
        {
            this.Engine = new ClientEngine();
            this.Managers = new ManagerHelper(this.Engine);
        }

        public ClientEngine Engine
        {
            get;
            private set;
        }

        public ManagerHelper Managers
        {
            get;
            private set;
        }


    
    }
}

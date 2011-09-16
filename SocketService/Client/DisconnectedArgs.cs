using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketService.Client
{
    public class DisconnectedArgs : EventArgs
    {
		public DisconnectedArgs()
		{
		}


		public DisconnectedArgs(Guid clientId)
		{
			ClientId = clientId;
		}

        public Guid ClientId
        {
            get;
            set;
        }

		//public Guid ClientId
		//{
		//    get;
		//    set;
		//}
    }
}

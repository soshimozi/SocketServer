using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketService.Framework.Net.Client
{
    public class ConnectArgs : EventArgs
    {
        public ConnectArgs()
        {
        }

		public ConnectArgs(Guid clientId, Socket rawSocket, string remoteAddress)
		{
            ClientId = clientId;
			RemoteAddress = remoteAddress;
            RawSocket = rawSocket;
		}
		
		public Guid ClientId
        {
            get;
            set;
        }

        public Socket RawSocket
        {
            get;
            set;
        }

        public string RemoteAddress
        {
            get;
            set;
        }

    }
}

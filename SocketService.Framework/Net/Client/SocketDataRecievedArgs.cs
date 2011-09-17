using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketService.Framework.Net.Client
{
    public class DataRecievedArgs : EventArgs
    {
		public DataRecievedArgs()
		{
		}

		public DataRecievedArgs(Guid clientId, byte [] data)
		{
			ClientId = clientId;
			Data = data;
		}

        public Guid ClientId { get; set; }
        public byte[] Data { get; set; }
    }
}

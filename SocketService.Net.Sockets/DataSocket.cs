using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace SocketService.SocketLib
{
    public class DataSocket : ZipSocket
    {
        public DataSocket(Socket socket) : base(socket)
        {
			ClientId = Guid.NewGuid();
        }

        public DataSocket(Socket socket, Guid clientId)
            : base(socket)
        {
            ClientId = clientId;
        }

		public Guid ClientId
		{
			get;
			set;
		}

    }
}

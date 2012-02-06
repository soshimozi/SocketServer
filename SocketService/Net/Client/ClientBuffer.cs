using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace SocketServer.Net.Client
{
    public class ClientBuffer
    {
        BinaryWriter writer;
        MemoryStream stream = new MemoryStream();


        public ClientBuffer()
        {
            writer = new BinaryWriter(stream);
        }

        public void Write(byte [] data)
        {
            writer.Write(data);
        }

        public byte[] Buffer
        {
            get
            {
                return stream.ToArray();
            }
        }

        public void Clear()
        {
            writer.Seek(0, SeekOrigin.Begin);
        }
    }
}

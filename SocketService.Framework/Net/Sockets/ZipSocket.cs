using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace SocketService.Framework.Net.Sockets
{
    public class ZipSocket
    {
        private Mutex _sendMutex = new Mutex();
        public ZipSocket(Socket socket, Guid clientId)
        {
            ClientId = clientId;
            RawSocket = socket;
            RemoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
        }

        public Socket RawSocket
        {
            get;
            private set;
        }

        public virtual void SendData(string data)
        {
            SendData(Encoding.UTF8.GetBytes(data));
        }

        public virtual void SendData(byte[] buffer)
        {
            _sendMutex.WaitOne();
            try
            {

                RawSocket.Send(Compress(buffer));
            }
            finally
            {
                _sendMutex.ReleaseMutex();
            }

        }

        public void Close()
        {
            RawSocket.Shutdown(SocketShutdown.Both);
            RawSocket.Close();
        }

        public bool Equals(Socket socket)
        {
            return socket == RawSocket;
        }

        public Guid ClientId
        {
            get;
            private set;
        }

        public string RemoteAddress
        {
            get;
            private set;
        }

        public byte[] ReceiveData()
        {
            byte[] zippedData = new byte[RawSocket.Available];
            RawSocket.Receive(zippedData);
            return Decompress(zippedData);
        }

        private byte[] Compress(byte[] data)
        {
            using (MemoryStream msData = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (GZipStream gz = new GZipStream(msData, CompressionMode.Compress))
                    {
                        byte[] bytes = new byte[4096];
                        int n;
                        while ((n = ms.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            gz.Write(bytes, 0, n);
                        }
                    }
                }

                return msData.ToArray();
            }

        }

        private byte[] Decompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (MemoryStream msOut = new MemoryStream())
                    {
                        byte[] bytes = new byte[4096];
                        int n;

                        while ((n = gs.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            msOut.Write(bytes, 0, n);
                        }

                        return msOut.ToArray();
                    }
                }
            }
        }

    }
}

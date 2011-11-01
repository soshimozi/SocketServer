using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace SocketService.Client.Core.Sockets
{
    public class ZipSocket
    {
        private readonly Mutex _sendMutex = new Mutex();
        /// <summary>
        /// Initializes a new instance of the <see cref="ZipSocket"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public ZipSocket(Socket socket)
        {
            RawSocket = socket;
            RemoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
        }

        /// <summary>
        /// Gets the raw socket.
        /// </summary>
        public Socket RawSocket
        {
            get;
            private set;
        }

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void SendData(string data)
        {
            SendData(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
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

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            RawSocket.Shutdown(SocketShutdown.Both);
            RawSocket.Close();
        }

        /// <summary>
        /// Determines whether the specified socket is equal.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <returns>
        ///   <c>true</c> if the specified socket is equal; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEqual(Socket socket)
        {
            return socket == RawSocket;
        }

        ///// <summary>
        ///// Gets the client id.
        ///// </summary>
        //public Guid ClientId
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Gets the remote address.
        /// </summary>
        public string RemoteAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the data.
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveData()
        {
            var zippedData = new byte[RawSocket.Available];
            RawSocket.Receive(zippedData);
            return Decompress(zippedData);
        }

        /// <summary>
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static byte[] Compress(byte[] data)
        {
            using (var msData = new MemoryStream())
            {
                using (var ms = new MemoryStream(data))
                {
                    using (var gz = new GZipStream(msData, CompressionMode.Compress))
                    {
                        var bytes = new byte[4096];
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

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static byte[] Decompress(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var gs = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (var msOut = new MemoryStream())
                    {
                        var bytes = new byte[4096];
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

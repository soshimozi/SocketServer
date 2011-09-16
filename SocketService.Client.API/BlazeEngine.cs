using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Response;
using SocketService.Net.Sockets;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using SocketService.Serial;

namespace SocketService.Client.API
{
    public class BlazeEngine
    {
        public event EventHandler<ConnectionResponse> Connected;

        private ZipSocket _clientSocket = null;
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);


        private class ServerInfo
        {
            public string Address;
            public int Port;
        }

        public void Connect(string address, int port)
        {
            Thread clientThread = new Thread(new ParameterizedThreadStart(ClientThread));
            clientThread.Start(new ServerInfo() { Address = address, Port = port });

        }

        public void ClientThread(object state)
        {
            ServerInfo info = state as ServerInfo;

            Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            rawSocket.Connect(info.Address, info.Port);

            // wrap the socket
            _clientSocket = new ZipSocket(rawSocket, Guid.NewGuid());

            while (!_stopEvent.WaitOne(0))
            {
                IList readList = new List<Socket>() { _clientSocket.RawSocket };

                // now let's wait for messages
                Socket.Select(readList, null, null, 100);

                object message = null;

                // there is only one socket in the poll list
                // so if the count is greater than 0 then
                // the only one available should be the client socket
                if (readList.Count > 0)
                {
                    if( _clientSocket.RawSocket.Available != 0 )
                    {
                        message = ReadObject(_clientSocket);

                        // handle message
                        HandleServerMessage(message);
                    }
                    //else
                    //{
                    // raise server disconnected event
                    //}
                }
            }
        }

        private void HandleServerMessage(object message)
        {
            if( message is 
        }

        private T ReadObject<T>(ZipSocket socket) where T : class
        {
            int availableBytes = socket.RawSocket.Available;
            if (availableBytes > 0)
            {
                byte[] objectData = socket.ReceiveData();

                // it should be a server message, we can look
                // at other message types later
                return ObjectSerialize.Deserialize<T>(objectData);
            }

            return null;
        }

        private object ReadObject(ZipSocket socket)
        {
            int availableBytes = socket.RawSocket.Available;
            if (availableBytes > 0)
            {
                byte[] objectData = socket.ReceiveData();

                // it should be a server message, we can look
                // at other message types later
                return ObjectSerialize.Deserialize(objectData);
            }

            return null;
        }

        public void Disconnect()
        {
            _stopEvent.Set();
        }

        protected void OnConnected(ConnectionResponse response)
        {
            var func = Connected;
            if (func != null)
            {
                func(this, response);
            }
        }
    }
}

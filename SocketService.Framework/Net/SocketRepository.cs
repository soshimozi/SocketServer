using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using SocketService.Framework.Net.Sockets;

namespace SocketService.Framework.Net
{
    public class SocketRepository
    {
        private readonly Dictionary<Guid, ZipSocket> _connectionList = new Dictionary<Guid, ZipSocket>();
        private readonly Mutex _connectionMutex = new Mutex();

        protected SocketRepository()
        {
        }

        private static SocketRepository _instance = null;

        public static SocketRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SocketRepository();
                }

                return _instance;
            }
        }

        public ZipSocket FindByClientId(Guid clientId)
        {
            _connectionMutex.WaitOne();
            try
            {
                if (_connectionList.ContainsKey(clientId))
                {
                    return _connectionList[clientId];
                }
                else
                {
                    return null;
                }

            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }

        public void Remove(Guid clientId)
        {
            _connectionMutex.WaitOne();
            try
            {
                _connectionList.Remove(clientId);
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }

        public void AddSocket(Guid clientId, Socket connection)
        {
            _connectionMutex.WaitOne();
            try
            {
                _connectionList.Add(clientId, new ZipSocket(connection, clientId));
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }


        public List<ZipSocket> ConnectionList
        {
            get
            {
                return _connectionList.Values.ToList();
            }
        }


    }
}

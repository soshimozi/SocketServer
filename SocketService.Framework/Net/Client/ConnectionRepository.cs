using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketService.Framework.Net.Client
{
    public class ConnectionRepository
    {
        private readonly List<Connection> _connectionList = new List<Connection>();
        private readonly Mutex _connectionMutex = new Mutex();

        protected ConnectionRepository()
        {
        }

        private static ConnectionRepository _instance = null;

        public static ConnectionRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConnectionRepository();
                }

                return _instance;
            }
        }

        public Connection FindConnectionByClientId(Guid clientId)
        {
            _connectionMutex.WaitOne();
            try
            {

                var q = from c in _connectionList
                        where c.ClientId == clientId
                        select c;

                return q.FirstOrDefault();
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }

        public void RemoveConnection(Connection connection)
        {
            _connectionMutex.WaitOne();
            try
            {
                _connectionList.Remove(connection);
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }

        public void AddConnection(Connection connection)
        {
            _connectionMutex.WaitOne();
            try
            {
                _connectionList.Add(connection);
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }


        public List<Connection> ConnectionList
        {
            get
            {
                _connectionMutex.WaitOne();
                try
                {
                }
                finally
                {
                    _connectionMutex.ReleaseMutex();
                }

                return _connectionList.ToList();
            }
        }

    }
}

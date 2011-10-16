using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SocketService.Net.Client
{
    public class ConnectionRepository
    {
        private static ConnectionRepository _instance;
        private readonly List<Connection> _connectionList = new List<Connection>();
        private readonly Mutex _connectionMutex = new Mutex();

        protected ConnectionRepository()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ConnectionRepository Instance
        {
            get { return _instance ?? (_instance = new ConnectionRepository()); }
        }

        /// <summary>
        /// Gets the connection list.
        /// </summary>
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

        /// <summary>
        /// Finds the connection by client id.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        public Connection FindConnectionByClientId(Guid clientId)
        {
            _connectionMutex.WaitOne();
            try
            {
                IEnumerable<Connection> q = from c in _connectionList
                                            where c.ClientId == clientId
                                            select c;

                return q.FirstOrDefault();
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
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

        /// <summary>
        /// Adds the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
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
    }
}
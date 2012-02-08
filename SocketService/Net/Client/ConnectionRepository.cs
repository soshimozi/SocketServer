using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SocketServer.Crypto;
using SocketServer.Shared;

namespace SocketServer.Net.Client
{
    public class ConnectionRepository
    {
        private static ConnectionRepository _instance;
        private readonly List<ClientConnection> _connectionList = new List<ClientConnection>();
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
        public List<ClientConnection> ConnectionList
        {
            get
            {
                _connectionMutex.WaitOne();
                try
                {
                    return _connectionList.ToList();
                }
                finally
                {
                    _connectionMutex.ReleaseMutex();
                }

            }
        }

        public IEnumerable<ClientConnection> Query(Func<ClientConnection, bool> filter)
        {
            return _connectionList.Where(filter);

        }

        /// <summary>
        /// Finds the connection by client id.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        //public ClientConnection FindConnectionByClientId(Guid clientId)
        //{
        //    _connectionMutex.WaitOne();
        //    try
        //    {
        //        IEnumerable<ClientConnection> q = from c in _connectionList
        //                                    where c.ClientId == clientId
        //                                    select c;

        //        return q.FirstOrDefault();
        //    }
        //    finally
        //    {
        //        _connectionMutex.ReleaseMutex();
        //    }
        //}

        /// <summary>
        /// Removes the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void RemoveConnection(ClientConnection connection)
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
        public ClientConnection NewConnection()
        {
            var connection 
                = new ClientConnection(
                    new ClientBuffer()
                    );

            _connectionMutex.WaitOne();
            try
            {
                _connectionList.Add(connection);
            }
            finally
            {
                _connectionMutex.ReleaseMutex();
            }

            return connection;
        }
    }
}
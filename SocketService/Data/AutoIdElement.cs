using System;

namespace SocketServer.Data
{
    public class AutoIdElement
    {
        private static readonly object LockObject = new object();

        private static long _nextId = -1;

        public static long GetNextID()
        {
            lock (LockObject)
            {
                if (_nextId == -1) _nextId = DateTime.UtcNow.Ticks; else _nextId++;
                return _nextId;
            }
        }
    }
}

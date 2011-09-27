using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Data
{
    public class DatabaseContextFactory
    {
        private static ServerDataEntities _context = null;
        public static ServerDataEntities Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new ServerDataEntities();
                }

                return _context;
            }
        }
    }
}

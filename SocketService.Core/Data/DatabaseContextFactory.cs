namespace SocketService.Core.Data
{
    public class DatabaseContextFactory
    {
        private static ServerDataEntities _context;
        public static ServerDataEntities Context
        {
            get { return _context ?? (_context = new ServerDataEntities()); }
        }
    }
}

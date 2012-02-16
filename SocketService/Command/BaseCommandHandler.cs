using System;
using log4net;
using System.Reflection;

namespace SocketServer.Command
{
    [Serializable]
    public abstract class BaseCommandHandler : ICommand
    {
        protected static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public abstract void Execute();
    }
}

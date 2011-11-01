using System.ServiceProcess;
using SocketService.Core.Messaging;
using SocketService.Net;

namespace SocketService
{
    public partial class SocketService : ServiceBase
    {
        private readonly SocketManager _serverManager = new SocketManager();
        private readonly MessageServer _messageServer = new MessageServer();

        public SocketService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            // TODO: Make port configurable
            _serverManager.StartServer();
            _messageServer.Start();
        }

        protected override void OnStop()
        {
            _serverManager.StopServer();
            _messageServer.Stop();
        }
    }
}

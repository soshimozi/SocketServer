using System;
using System.Windows.Forms;
using SocketServer.Core.Messaging;
using SocketServer.Net;
using SocketServer.Core.Configuration;
using System.Configuration;

namespace SocketServer
{
    public partial class ServerControlForm : Form
    {
        private readonly SocketManager _serverManager;
        private readonly MessageServer _messageServer = new MessageServer();

        public ServerControlForm()
        {
            RequestHandlerConfigurationSection config = 
                (RequestHandlerConfigurationSection)ConfigurationManager.
                GetSection("HandlersSection");

            _serverManager = new SocketManager(config);

            InitializeComponent();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            _messageServer.Start();
            _serverManager.StartServer();
        }

        private void StopButtonClick(object sender, EventArgs e)
        {
            _serverManager.StopServer();
            _messageServer.Stop();
        }

        private void ServerControlFormFormClosing(object sender, FormClosingEventArgs e)
        {
            _serverManager.StopServer();
            _messageServer.Stop();
        }

    }
}

using System;
using System.Windows.Forms;
using SocketServer.Messaging;
using SocketServer.Net;
using SocketServer.Configuration;
using System.Configuration;

namespace SocketServer
{
    public partial class ServerControlForm : Form
    {
        private readonly SocketManager _serverManager;
        private readonly MessageServer _messageServer;// = new MessageServer();

        public ServerControlForm()
        {
            SocketServerConfiguration config = ServerConfigurationHelper.GetServerConfiguration();

            _serverManager = new SocketManager(config);


            if (config.Queues.Count > 0)
            {
                _messageServer = new MessageServer(config.Queues[0].QueueName, config.Queues[0].QueuePath);
            }

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

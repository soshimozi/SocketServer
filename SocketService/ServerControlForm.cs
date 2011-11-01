using System;
using System.Windows.Forms;
using SocketService.Core.Messaging;
using SocketService.Net;

namespace SocketService
{
    public partial class ServerControlForm : Form
    {
        private readonly SocketManager _serverManager = new SocketManager();
        private readonly MessageServer _messageServer = new MessageServer();

        public ServerControlForm()
        {
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using SocketService.Request;
using SocketService.Net;
using SocketService.Messaging;

namespace SocketService
{
    public partial class SocketService : ServiceBase
    {
        private readonly SocketManager serverManager = new SocketManager();
        private readonly MessageServer messageServer = new MessageServer();

        public SocketService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            // TODO: Make port configurable
            serverManager.StartServer(4000);
            messageServer.Start();
        }

        protected override void OnStop()
        {
            serverManager.StopServer();
            messageServer.Stop();
        }
    }
}

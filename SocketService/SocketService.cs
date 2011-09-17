using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Net;

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
            serverManager.StartServer();
            messageServer.Start();
        }

        protected override void OnStop()
        {
            serverManager.StopServer();
            messageServer.Stop();
        }
    }
}

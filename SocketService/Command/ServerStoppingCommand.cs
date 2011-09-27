using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Actions;
using SocketService.Net;

namespace SocketService.Command
{
    [Serializable]
    public class ServerStoppingCommand : BaseMessageHandler
    {
        private readonly SocketServer _socketServer;
        public ServerStoppingCommand(SocketServer socketServer)
        {
            _socketServer = socketServer;
        }

        public override void Execute()
        {
            // remove all non-persistent rooms
            RoomActionEngine.Instance.RemoveNonPersistentRooms();

            // remove all users
            UserActionEngine.Instance.RemoveAllUsers();

            _socketServer.StopServer();
        }
    }
}

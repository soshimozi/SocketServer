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
    public class ServerStartingCommand : BaseMessageHandler
    {
        public override void Execute()
        {
            // remove all users first
            UserActionEngine.Instance.RemoveAllUsers();

            // remove all non-persistent rooms
            RoomActionEngine.Instance.RemoveNonPersistentRooms();

        }
    }
}

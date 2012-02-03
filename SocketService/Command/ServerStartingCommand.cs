using System;
using SocketServer.Actions;
using SocketServer.Core.Messaging;

namespace SocketServer.Command
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
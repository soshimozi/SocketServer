using System;
using SocketService.Actions;
using SocketService.Core.Messaging;

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
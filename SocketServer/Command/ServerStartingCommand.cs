using System;
using SocketServer.Actions;

namespace SocketServer.Command
{
    [Serializable]
    public class ServerStartingCommand : BaseCommandHandler
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.SharedObjects;
using SocketService.Framework.Actions;
using SocketService.Framework.Messaging;

namespace SocketService.Command
{
    [Serializable]
    class CreateRoomVariableCommand : BaseMessageHandler
    {
        private readonly string _room;
        private readonly string _name;
        private readonly ServerObject _so;
        public CreateRoomVariableCommand(string room, string name, ServerObject so)
        {
            _room = room;
            _name = name;
            _so = so;
        }

        public override void Execute()
        {
            RoomActionEngine.Instance.CreateRoomVariable(_room, _name, _so);
        }
    }
}

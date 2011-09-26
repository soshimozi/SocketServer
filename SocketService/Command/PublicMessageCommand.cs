using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Client.Event;
using SocketService.Framework.Data;
using SocketService.Framework.Client.API;

namespace SocketService.Command
{
    [Serializable]
    public class PublicMessageCommand : BaseMessageHandler
    {
        private readonly string _zone;
        private readonly string _room;
        private readonly string _user;
        private readonly string _message;
        public PublicMessageCommand(string Zone, string Room, string User, string Message)
        {
            _zone = Zone;
            _room = Room;
            _user = User;
            _message = Message;
        }

        public override void Execute()
        {
            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(UserRepository.Instance.FindClientKeysByRoom(_room).ToArray(),
                    new PublicMessageEvent() {  Zone = _zone, Room = _room, UserName = _user, Message = _message }
                )
            );

            // check if room has any plugins, call the plugin UserSendPublicMessage event
            Room room = RoomRepository.Instance.FindByName(_room);
            if (room != null)
            {
                ChainAction lastAction = ChainAction.NoAction;
                foreach (IPlugin plugin in room.GetRoomPlugins())
                {
                    lastAction = plugin.UserSendPublicMessage(new UserPublicMessageContext() { Zone = _zone, Room = _room, User = _user, Message = _message });
                    if ( lastAction == ChainAction.Fail || lastAction == ChainAction.Stop)
                    {
                        // we are done
                        break;
                    }
                }

                // now check for last action
                if (lastAction == ChainAction.Fail)
                {
                    // we had a fail action, handle it somehow?
                }
            }

        }
    }
}

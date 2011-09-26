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
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _user;
        private readonly string _message;
        public PublicMessageCommand(int ZoneId, int RoomId, string User, string Message)
        {
            _zoneId = ZoneId;
            _roomId = RoomId;
            _user = User;
            _message = Message;
        }

        public override void Execute()
        {
            //Room room = 
            //MSMQQueueWrapper.QueueCommand(
            //    new BroadcastObjectCommand(UserRepository.Instance.Find(.ToArray(),
            //        new PublicMessageEvent() { ZoneId = _zoneId, RoomId = _roomId, UserName = _user, Message = _message }
            //    )
            //);

            //// check if room has any plugins, call the plugin UserSendPublicMessage event
            //Room room = RoomRepository.Instance.FindByName(_room);
            //if (room != null)
            //{
            //    ChainAction lastAction = ChainAction.NoAction;
            //    foreach (IPlugin plugin in room.GetRoomPlugins())
            //    {
            //        lastAction = plugin.UserSendPublicMessage(new UserPublicMessageContext() { Zone = _zone, Room = _room, User = _user, Message = _message });
            //        if ( lastAction == ChainAction.Fail || lastAction == ChainAction.Stop)
            //        {
            //            // we are done
            //            break;
            //        }
            //    }

            //    // now check for last action
            //    if (lastAction == ChainAction.Fail)
            //    {
            //        // we had a fail action, handle it somehow?
            //    }
            //}

        }
    }
}

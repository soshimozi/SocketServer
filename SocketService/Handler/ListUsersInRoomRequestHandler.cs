using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Command;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Request;

namespace SocketService
{
    [Serializable()]
    [ServiceHandlerType(typeof(ListUsersInRoomRequest))]
    public class ListUsersInRoomRequestHandler : BaseHandler<ListUsersInRoomRequest, Guid>
    {
        public override bool HandleRequest(ListUsersInRoomRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new ListUsersInRoomCommand(request.RoomName, state)
            );

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceHandlerLib;
using System.ComponentModel.Composition;
using BlazeRequestResponse;
using BlazeServerData;

namespace ServiceHandlers
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(PublicMessageRequest))]
    class PublicMessageRequestHandler : IServiceHandler
    {
        private UserManager _userManager;

        public PublicMessageRequestHandler()
        {
            _userManager = new UserManager(DataEntityFactory.GetDataEntities());
        }

        public bool HandleRequest(IRequest request, IServerContext server)
        {
            PublicMessageRequest messageRequest = request as PublicMessageRequest;

            if (messageRequest != null)
            {
                PublicMessageEvent messageEvent =
                    new PublicMessageEvent();

                messageEvent.BzObject = messageRequest.BzObject;
                messageEvent.Message = messageRequest.Message;
                messageEvent.RoomId = messageRequest.RoomId;
                messageEvent.ZoneId = messageRequest.ZoneId;

                int? userId = server.GetUserIdForClientId(request.ClientId);
                User user = _userManager.Search((u) => u.UserId == userId);

                if (user != null)
                {
                    messageEvent.UserName = user.UserName;
                }

                messageEvent.Message = messageRequest.Message;

                server.BroadcastObject(messageEvent, request.ClientId);
                return true;
            }

            return false;
        }
    }
}

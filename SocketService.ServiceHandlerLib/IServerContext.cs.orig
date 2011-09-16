using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceHandlerLib
{
    public interface IServerContext
    {
        void AssociateClientWithUser(Guid clientId, int userId);
        void BroadcastObject(object objectToBroadcast, Guid? clientToSkip=null);
        int? GetUserIdForClientId(Guid clientId);
        void SendObject(Object objectToSend, Guid clientId);
    }
}

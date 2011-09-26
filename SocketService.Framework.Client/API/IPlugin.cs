using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.API
{
    public interface IPlugin
    {
        ChainAction UserSendPublicMessage(UserPublicMessageContext context);
    }

    public enum ChainAction
    {
        Fail,
        Continue,
        Stop,
        NoAction
    }
}

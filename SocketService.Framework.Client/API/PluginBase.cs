using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace SocketService.Framework.Client.API
{
    [InheritedExport(typeof(IPlugin))]
    public class PluginBase : IPlugin
    {
        public virtual ChainAction UserSendPublicMessage(UserPublicMessageContext context)
        {
            return ChainAction.NoAction;
        }
    }
}

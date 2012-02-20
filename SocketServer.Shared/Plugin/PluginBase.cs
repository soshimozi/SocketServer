using System.ComponentModel.Composition;

namespace SocketServer.Shared.Plugin
{
    [InheritedExport(typeof(IPlugin))]
    public class PluginBase : IPlugin
    {
        public virtual ChainAction UserSendPublicMessage(object context)
        {
            return ChainAction.NoAction;
        }
    }
}

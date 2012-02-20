using System;
using System.ComponentModel.Composition;

namespace SocketServer.Shared.Plugin
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class PluginMetaDataAttribute : ExportAttribute, IPluginMetaData
    {
    }
}

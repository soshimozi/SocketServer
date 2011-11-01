using System;
using System.ComponentModel.Composition;

namespace SocketService.Shared.Plugin
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class PluginMetaDataAttribute : ExportAttribute, IPluginMetaData
    {
    }
}

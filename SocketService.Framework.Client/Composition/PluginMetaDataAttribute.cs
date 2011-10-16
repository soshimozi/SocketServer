using System;
using System.ComponentModel.Composition;

namespace SocketService.Framework.Client.Composition
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class PluginMetaDataAttribute : ExportAttribute, IPluginMetaData
    {
    }
}

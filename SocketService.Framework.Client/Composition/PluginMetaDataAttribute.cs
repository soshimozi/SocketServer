using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace SocketService.Framework.Client.Composition
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class PluginMetaDataAttribute : ExportAttribute, IPluginMetaData
    {
    }
}

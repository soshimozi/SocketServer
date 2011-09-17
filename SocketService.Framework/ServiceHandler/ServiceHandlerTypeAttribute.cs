using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace SocketService.Framework.ServiceHandler
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class ServiceHandlerTypeAttribute : ExportAttribute, IServiceHandlerMetaData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHandlerTypeAttribute"/> class.
        /// </summary>
        public ServiceHandlerTypeAttribute() 
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHandlerTypeAttribute"/> class.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        public ServiceHandlerTypeAttribute(Type handlerType) 
            : base(typeof(IServiceHandler))
        {
            HandlerType = handlerType;
        }

        #region IHandlerType Members

        public Type HandlerType
        {
            get;
            set;
        }

        #endregion
    }
}

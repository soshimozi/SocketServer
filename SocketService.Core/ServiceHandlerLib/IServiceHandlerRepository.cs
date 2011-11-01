using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.ServiceHandlerLib
{
    public interface IServiceHandlerRepository
    {
        /// <summary>
        /// Gets the handler list by type.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns></returns>
        List<IServiceHandler> GetHandlerListByType(Type type);

        /// <summary>
        /// Loads the handlers.
        /// </summary>
        /// <param name="handlerPath">The handler path.</param>
        void LoadHandlers();

    }
}

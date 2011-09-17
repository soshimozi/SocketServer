using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.IO;
using System.Configuration;
using SocketService.Framework.Configuration;
using SocketService.Framework.ServiceHandler;

namespace SocketService
{
    public class ServiceHandlerRepository : IServiceHandlerRepository
    {
        [ImportMany]
        protected IEnumerable<Lazy<IServiceHandler, IServiceHandlerMetaData>> _handlerList;

        private static ServiceHandlerRepository _instance = null;

        /// <summary>
        /// Initializes the <see cref="ServiceHandlerRepository"/> class.
        /// </summary>
        static ServiceHandlerRepository()
        {
            Instance.LoadHandlers();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ServiceHandlerRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceHandlerRepository();
                }

                return _instance;
            }
        }

        protected ServiceHandlerRepository()
        {
            //string handlerPath
        }

        /// <summary>
        /// Gets the handler list by type.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns></returns>
        public List<IServiceHandler> GetHandlerListByType(Type type)
        {
            List<IServiceHandler> serviceHandlers = new List<IServiceHandler>();

            serviceHandlers = _handlerList
                                .Where((h) => h.Metadata.HandlerType == type)
                                .Select((lz) => lz.Value)
                                .ToList();

            return serviceHandlers;
        }

        /// <summary>
        /// Loads the handlers.
        /// </summary>
        /// <param name="handlerPath">The handler path.</param>
        public void LoadHandlers()
        {
            SocketServiceConfiguration config = null;

            try
            { config = (SocketServiceConfiguration)ConfigurationManager.GetSection("SocketServerConfiguration"); }
            catch (Exception ex)
            { }

            var aggregateCatalog = new AggregateCatalog();

            Assembly callingAssembly = Assembly.GetCallingAssembly();

            // an assembly catalog to load information about parts from this assembly
            var assemblyCatalog = new AssemblyCatalog(callingAssembly);

            if (config != null)
            {
                foreach (PluginInfoInstanceElement pi in config.Plugins)
                {
                    var directoryCatalog = new DirectoryCatalog(pi.Path, "*.dll");
                    aggregateCatalog.Catalogs.Add(directoryCatalog);
                }
            }


            aggregateCatalog.Catalogs.Add(assemblyCatalog);

            // create a container for our catalogs
            var container = new CompositionContainer(aggregateCatalog);

            // finally, compose the parts
            container.ComposeParts(this);
        }
    }
}

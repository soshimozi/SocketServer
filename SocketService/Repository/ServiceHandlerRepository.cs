using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using SocketService.Core.ServiceHandlerLib;

namespace SocketService.Repository
{
    public class ServiceHandlerLookup // : IServiceHandlerRepository
    {
        private static ServiceHandlerLookup _instance;
        [ImportMany] protected IEnumerable<Lazy<IServiceHandler, IServiceHandlerMetaData>> HandlerList;

        /// <summary>
        /// Initializes the <see cref="ServiceHandlerLookup"/> class.
        /// </summary>
        static ServiceHandlerLookup()
        {
            Instance.LoadHandlers();
        }

        protected ServiceHandlerLookup()
        {
            //string handlerPath
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ServiceHandlerLookup Instance
        {
            get { return _instance ?? (_instance = new ServiceHandlerLookup()); }
        }

        /// <summary>
        /// Gets the handler list by type.
        /// </summary>
        /// <returns></returns>
        public List<IServiceHandler> GetHandlerListByType(Type type)
        {
            List<IServiceHandler> serviceHandlers = HandlerList
                .Where(h => h.Metadata.HandlerType == type)
                .Select(lz => lz.Value)
                .ToList();

            return serviceHandlers;
        }

        /// <summary>
        /// Loads the handlers.
        /// </summary>
        public void LoadHandlers()
        {
            //try
            //{
            //    (SocketServiceConfiguration) ConfigurationManager.GetSection("SocketServerConfiguration");
            //}
            //catch (Exception)
            //{
            //}

            var aggregateCatalog = new AggregateCatalog();

            var callingAssembly = Assembly.GetExecutingAssembly();

            // an assembly catalog to load information about parts from this assembly
            var assemblyCatalog = new AssemblyCatalog(callingAssembly);

            var assemblyLocation = Path.GetDirectoryName(callingAssembly.Location);
            if (assemblyLocation != null)
            {
                var directoryCatalog = new DirectoryCatalog(assemblyLocation, "*.dll");

                aggregateCatalog.Catalogs.Add(assemblyCatalog);
                aggregateCatalog.Catalogs.Add(directoryCatalog);
            }

            // create a container for our catalogs
            var container = new CompositionContainer(aggregateCatalog);

            // finally, compose the parts
            container.ComposeParts(this);
        }
    }
}
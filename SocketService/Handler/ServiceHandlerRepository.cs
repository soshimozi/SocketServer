using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.IO;
using SocketService.ServiceHandler;

namespace SocketService.Handler
{
    public class ServiceHandlerRepository
    {
        [ImportMany]
        protected IEnumerable<Lazy<IServiceHandler, IServiceHandlerType>> _handlerList;

        private static ServiceHandlerRepository _instance = null;

        static ServiceHandlerRepository()
        {
            Instance.LoadHandlers("");
        }

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

        public List<IServiceHandler> GetHandlerListForType(Type t)
        {
            List<IServiceHandler> serviceHandlers = new List<IServiceHandler>();

            serviceHandlers = _handlerList
                                .Where((h) => h.Metadata.HandlerType == t)
                                .Select((lz) => lz.Value)
                                .ToList();

            return serviceHandlers;
        }

        public void LoadHandlers(string handlerPath)
        {
            var aggregateCatalog = new AggregateCatalog();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            // an assembly catalog to load information about parts from this assembly
            var assemblyCatalog = new AssemblyCatalog(executingAssembly);

            if (!string.IsNullOrEmpty(handlerPath))
            {
                var directoryCatalog = new DirectoryCatalog(handlerPath, "*.dll");
                aggregateCatalog.Catalogs.Add(directoryCatalog);
            }

            aggregateCatalog.Catalogs.Add(assemblyCatalog);
            //aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));

            // create a container for our catalogs
            var container = new CompositionContainer(aggregateCatalog);

            // finally, compose the parts
            container.ComposeParts(this);
        }


    }
}

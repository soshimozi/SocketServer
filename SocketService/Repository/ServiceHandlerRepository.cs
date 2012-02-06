using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SocketServer.Configuration;
using System.Configuration;
using SocketServer.Reflection;
using log4net;
using SocketServer.Handler;

namespace SocketServer.Repository
{
    public class ServiceHandlerLookup // : IServiceHandlerRepository
    {
        private Dictionary<string, ServiceHandler> _handlerMap = new Dictionary<string, ServiceHandler>();
        private static ServiceHandlerLookup _instance;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes the <see cref="ServiceHandlerLookup"/> class.
        /// </summary>
        static ServiceHandlerLookup()
        {
            SocketServerConfiguration config =
                (SocketServerConfiguration)ConfigurationManager.
                GetSection("SocketServerConfiguration");

            Instance.LoadHandlers(config);
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
        //public List<IServiceHandler> GetHandlerListByType(Type type)
        //{
        //    List<IServiceHandler> serviceHandlers = HandlerList
        //        .Where(h => h.Metadata.HandlerType == type)
        //        .Select(lz => lz.Value)
        //        .ToList();

        //    return serviceHandlers;
        //}

        /// <summary>
        /// Loads the handlers.
        /// </summary>
        //public void LoadHandlers()
        //{
            ////try
            ////{
            ////    (SocketServiceConfiguration) ConfigurationManager.GetSection("SocketServerConfiguration");
            ////}
            ////catch (Exception)
            ////{
            ////}

            //var aggregateCatalog = new AggregateCatalog();

            //var callingAssembly = Assembly.GetExecutingAssembly();

            //// an assembly catalog to load information about parts from this assembly
            //var assemblyCatalog = new AssemblyCatalog(callingAssembly);

            //var assemblyLocation = Path.GetDirectoryName(callingAssembly.Location);
            //if (assemblyLocation != null)
            //{
            //    var directoryCatalog = new DirectoryCatalog(assemblyLocation, "*.dll");

            //    aggregateCatalog.Catalogs.Add(assemblyCatalog);
            //    aggregateCatalog.Catalogs.Add(directoryCatalog);
            //}

            //// create a container for our catalogs
            //var container = new CompositionContainer(aggregateCatalog);

            //// finally, compose the parts
            //container.ComposeParts(this);
        //}

        private void LoadHandlers(SocketServerConfiguration config)
        {
            foreach (RequestHandlerConfigurationElement element in config.Handlers)
            {
                string key = element.Key;

                Type requestType = ReflectionHelper.FindType(element.RequestType);
                Type handlerType = ReflectionHelper.FindType(element.HandlerType);

                if (handlerType != null && requestType != null)
                {
                    // create an instance of the handler
                    object handler = ReflectionHelper.ActivateObject(handlerType, null);
                    if (handler != null)
                    {
                        Type interfaceType = ReflectionHelper.FindGenericInterfaceMethod("IRequestHandler", new Type[] { requestType }, handlerType);
                        if (interfaceType != null)
                        {
                            // we are in business
                            try
                            {
                                MethodInfo mi = interfaceType.GetMethod("HandleRequest");
                                if (mi != null)
                                {
                                    ServiceHandler helper = new ServiceHandler(handler, mi);
                                    _handlerMap.Add(key, helper);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ErrorFormat("Error in HandleRequest\n. {0}", ex);
                            }
                        }

                        Logger.InfoFormat("Adding handler for request tag {0} - handler type ({1}), request type ({2})", element.Key, element.HandlerType, element.RequestType);
                    }
                }
            }
        }

        public void InvokeHandler(string key, params object [] parameters)
        {
            if (_handlerMap.ContainsKey(key))
            {
                _handlerMap[key].Invoke(parameters);
            }
        }
    }
}
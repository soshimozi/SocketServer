﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.IO;
using System.Configuration;
using SocketService.Framework.Configuration;
using SocketService.Framework.ServiceHandlerLib;

namespace SocketService
{
    public class ServiceHandlerLookup // : IServiceHandlerRepository
    {
        [ImportMany]
        protected IEnumerable<Lazy<IServiceHandler, IServiceHandlerMetaData>> _handlerList;

        private static ServiceHandlerLookup _instance = null;

        /// <summary>
        /// Initializes the <see cref="ServiceHandlerLookup"/> class.
        /// </summary>
        static ServiceHandlerLookup()
        {
            Instance.LoadHandlers();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ServiceHandlerLookup Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceHandlerLookup();
                }

                return _instance;
            }
        }

        protected ServiceHandlerLookup()
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
            catch (Exception)
            { }

            var aggregateCatalog = new AggregateCatalog();

            Assembly callingAssembly = Assembly.GetExecutingAssembly();

            // an assembly catalog to load information about parts from this assembly
            var assemblyCatalog = new AssemblyCatalog(callingAssembly);
            var directoryCatalog = new DirectoryCatalog(Path.GetDirectoryName(callingAssembly.Location), "*.dll");

            aggregateCatalog.Catalogs.Add(assemblyCatalog);
            aggregateCatalog.Catalogs.Add(directoryCatalog);

            // create a container for our catalogs
            var container = new CompositionContainer(aggregateCatalog);

            // finally, compose the parts
            container.ComposeParts(this);
        }
    }
}
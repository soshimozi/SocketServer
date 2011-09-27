using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace SocketService.Framework
{
    public class DependencyFactory
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }

            private set
            {
                _container = value;
            }
        }

        static DependencyFactory()
        {
            var container = new UnityContainer();

            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (section != null)
            {
                section.Configure(container);
            }

            Container = container;
        }

        public static T Resolve<T>()
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>();
            }

            return ret;
        }
    }
}

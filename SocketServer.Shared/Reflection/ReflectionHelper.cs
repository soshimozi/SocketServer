using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace SocketServer.Shared.Reflection
{
    public static class ReflectionHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Type FindGenericInterfaceMethod(string baseInterfaceTypeName, Type [] genericParameters, Type type)
        {
            string interfaceName = string.Format("{0}`{1}", baseInterfaceTypeName, genericParameters.Length);

            Type interfaceType = type.GetInterface(interfaceName);
            if (interfaceType != null)
            {
                Type[] genericTypes = interfaceType.GetGenericArguments();
                foreach(Type genericType in genericParameters)
                {
                    if (!genericTypes.Contains(genericType))
                    {
                        return null;
                    }
                }
            }

            return interfaceType;
        }

        public static object ActivateObject(Type type, params object [] parameters)
        {
            try
            {
                return Activator.CreateInstance(type, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public static object InvokeStaticMethodOnType(Type type, string methodName, params object [] parameters)
        {
            var staticMethod = type.GetMethod(methodName,
                  BindingFlags.Static | BindingFlags.Public,
                  null,
                  parameters.Select( x => x.GetType() ).ToArray(),
                  null);

            if (staticMethod != null)
            {
                return staticMethod.Invoke(null, parameters);
            }

            throw new MissingMethodException(methodName);
        }

        public static Type FindTypeFullSearch(string typeName)
        {
            AssemblyName[] assemblyList = Assembly.GetEntryAssembly().GetReferencedAssemblies();

            foreach (AssemblyName name in assemblyList)
            {
                Assembly assembly = Assembly.Load(name);

                Type[] types = assembly.GetTypes();

                Type t = types.FirstOrDefault(x => x.FullName == typeName);
                if (t != null)
                    return t;

            }

            return null;
        }

        public static Type FindType(string typename)
        {
            string[] parts = typename.Split(",".ToCharArray());

            Type type = null;
            if (parts.Length > 1)
            {
                Assembly assembly = null;

                // we can have a list of directories to search for the file
                // before we try to load the assembly
                // iterate through each directory entry
                // look for file in directory, if found then
                // build path to assembly and use that in the
                // Assembly.Load method
                try
                {
                    assembly = Assembly.Load(new AssemblyName(parts[1].Trim()));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                if (assembly != null)
                {
                    try
                    {
                        type = assembly.GetType(parts[0].Trim(), false, false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

            }
            else
            {
                // let's find the type in the current assembly
                type = Assembly.GetExecutingAssembly().GetType(typename, false, false);
            }

            return type;
        }
    
    }
}

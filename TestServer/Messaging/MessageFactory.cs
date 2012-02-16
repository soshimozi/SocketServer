using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;

namespace TestServer.Messaging
{
    public class MessageFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MessageFactory));

        private static readonly Dictionary<string, Type> messageMap = new Dictionary<string, Type>();

        /// <summary>
        /// Creates a new instance of a class that implements IMessage
        /// </summary>
        /// <param name="messageName">The registered name of the message to be created</param>
        /// <returns>An IMessage object, or null if message does not exist</returns>
        public static IMessage Create(string messageName)
        {
            lock (messageMap)
            {
                string nameUpper = messageName.ToUpper();
                if (messageMap.ContainsKey(nameUpper))
                    return Activator.CreateInstance(messageMap[nameUpper]) as IMessage;

                return null;
            }
        }

        /// <summary>
        /// Registers a new IMessage-derived class as available to be created by the factory
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="t">The IMessage-derived type to be registered</param>
        public static void Register(string messageName, Type t)
        {
            if (t == null) return;
            lock (messageMap)
            {
                if (messageMap.ContainsKey(messageName.ToUpper())) return;
                messageMap.Add(messageName.ToUpper(), t);
                logger.Logger.Log(null, Level.Finer, "Registered Message: " + messageName, null); // TODO: change to logger.Log
            }
        }

        public static void Register(Type t)
        {
            try
            {
                string messageName = ((IMessage)Activator.CreateInstance(t)).Name;
                Register(messageName, t);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Type " + t + " does not implement IMessage");
            }
        }

        public static void Remove(string messageName)
        {
            lock (messageMap)
            {
                if (messageMap.ContainsKey(messageName.ToUpper()))
                    messageMap.Remove(messageName.ToUpper());
            }
        }

        public static void Remove(Type t)
        {
            try
            {
                string messageName = ((IMessage)Activator.CreateInstance(t)).Name.ToUpper();
                Remove(messageName);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Type " + t + " does not implement IMessage");
            }
        }

        public static void RemoveAll()
        {
            lock (messageMap)
            {
                messageMap.Clear();
            }
        }
    }
}

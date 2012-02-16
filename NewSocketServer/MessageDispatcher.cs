using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Configuration;
using log4net;
using System.Net;
using System.Reflection;

namespace SocketServer
{
    public class MessageDispatcher
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MessageDispatcher));

        #region  MessageHandler definition
        private class MessageHandler
        {
            public MessageHandler(MessageHandlerDelegate handler, object target, bool isPrimary, bool needsSourceChannel, bool needsEndpointParameter)
            {
                Target = target;
                Handler = handler;
                IsPrimaryHandler = isPrimary;
                NeedsSourceChannel = needsSourceChannel;
                NeedsSourceEndpoint = needsEndpointParameter;
            }

            public MessageHandlerDelegate Handler { get; private set; }

            public bool IsPrimaryHandler { get; private set; }

            public bool NeedsSourceChannel { get; private set; }

            public bool NeedsSourceEndpoint { get; private set; }

            public object Target { get; private set; }

            public IMessage Execute(IMessage msg, object param)
            {
                if (NeedsSourceChannel)
                    return (IMessage)Handler(Target, new[] { msg, param });
                if (NeedsSourceEndpoint)
                    return (IMessage)Handler(Target, new[] { msg, param });
                return (IMessage)Handler(Target, new object[] { msg });
            }

            public override bool Equals(object obj)
            {
                MessageHandler handler = (MessageHandler)obj;

                return Target == handler.Target &&
                       IsPrimaryHandler == handler.IsPrimaryHandler &&
                       NeedsSourceChannel == handler.NeedsSourceChannel &&
                       NeedsSourceEndpoint == handler.NeedsSourceEndpoint;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        #endregion

        private readonly Dictionary<string, List<MessageHandler>> messageHandlers = new Dictionary<string, List<MessageHandler>>();

        public MessageDispatcher()
        {
        }

        public MessageDispatcher(IMessageHandler handler)
        {
            RegisterHandlers(handler);
        }

        /// <summary>
        /// Gets the number of messages for which handlers have been registered
        /// </summary>
        public int HandlerCount
        {
            get
            {
                int count = 0;
                foreach (List<MessageHandler> list in messageHandlers.Values)
                {
                    count += list.Count;
                }
                return count;
            }
        }

        public void AddNullHandler(string messageName)
        {
            if (!messageHandlers.ContainsKey(messageName))
                messageHandlers.Add(messageName, new List<MessageHandler>());
        }

        public void RegisterHandlers(IMessageHandler targetObject)
        {
            Type handlerClassType = targetObject.GetType();
            MethodInfo[] methods = handlerClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo info in methods)
            {
                // We only care about methods
                if (info.MemberType != MemberTypes.Method)
                    continue;

                // Check to make sure that the MessageHandlerAttribute is set
                object[] attributes = info.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
                if (attributes != null && attributes.Length >= 1)
                {
                    // We need to get the Message Type through further reflection
                    ParameterInfo[] methodParams = info.GetParameters();
                    if (methodParams == null || methodParams.Length == 0)
                        continue; // no parameters, no message handler...  move on...

                    // We will simply assume that the first parameter is the message
                    // SHOULD WE THROW AN EXCEPTION IF PARAMETER COUNT IS 0 OR MORE THAN 1 ???
                    Type messageType = methodParams[0].ParameterType;
                    string messageName = ((IMessage)Activator.CreateInstance(messageType)).Name;
                    // Add the delegate to the list if it doesn't exist
                    if (!messageHandlers.ContainsKey(messageName))
                    {
                        messageHandlers.Add(messageName, new List<MessageHandler>());
                    }

                    // We allow a second parameter to the handler if it is a Channel, so that the method can
                    // know the source of the message.
                    bool needsChannelParameter = false;
                    bool needsEndpointParameter = false;
                    if (methodParams.Length == 2)
                    {
                        if (methodParams[1].ParameterType == typeof(ClientConnection))
                        {
                            needsChannelParameter = true;
                        }
                        else if (methodParams[1].ParameterType == typeof(IPEndPoint))
                        {
                            needsEndpointParameter = true;
                        }
                    }

                    List<MessageHandler> methodList = messageHandlers[messageName];

                    MessageHandlerAttribute attribute = (MessageHandlerAttribute)attributes[0];
                    MessageHandler messageHandler =
                        new MessageHandler(FastMethodInvoker.GetMethodInvoker(targetObject, info),
                                           targetObject,
                                           attribute.PrimaryHandler,
                                           needsChannelParameter, needsEndpointParameter);

                    // Make sure there is only 1 primary handler per message
                    // We'll make sure the primary handler is the first in the list, for easy checking
                    if (messageHandler.IsPrimaryHandler && methodList.Count > 0 && methodList[0].IsPrimaryHandler)
                        throw new InvalidOperationException("Cannot have multiple primary handlers for message: " + messageName);

                    // Add the handler to the list.  Front of list for primary handlers, back of list for non
                    if (messageHandler.IsPrimaryHandler)
                        methodList.Insert(0, messageHandler);
                    else
                        methodList.Add(messageHandler);

                    // Register message class with MessageFactory
                    MessageFactory.Register(messageType);

                }
            }
        }

        public void UnregisterHandlers(IMessageHandler handler)
        {
            Type handlerClassType = handler.GetType();
            MethodInfo[] methods = handlerClassType.GetMethods();
            foreach (MethodInfo info in methods)
            {
                // We only care about methods
                if (info.MemberType != MemberTypes.Method)
                    continue;

                // Check to make sure that the MessageHandlerAttribute is set
                object[] attributes = info.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
                if (attributes != null && attributes.Length >= 1)
                {
                    // We need to get the Message Type through further reflection
                    ParameterInfo[] methodParams = info.GetParameters();
                    if (methodParams == null || methodParams.Length == 0)
                        continue; // no parameters, no message handler...  move on...

                    // We will simply assume that the first parameter is the message
                    // SHOULD WE THROW AN EXCEPTION IF PARAMETER COUNT IS 0 OR MORE THAN 1 ???
                    Type messageType = methodParams[0].ParameterType;
                    string messageName = ((IMessage)Activator.CreateInstance(messageType)).Name;

                    // Remove the handler from the list if it exists
                    if (!messageHandlers.ContainsKey(messageName))
                        continue;

                    bool needsChannelParameter = false;
                    bool needsEndpointParameter = false;
                    if (methodParams.Length == 2)
                    {
                        if (methodParams[1].ParameterType == typeof(ClientConnection))
                        {
                            needsChannelParameter = true;
                        }
                        else if (methodParams[1].ParameterType == typeof(IPEndPoint))
                        {
                            needsEndpointParameter = true;
                        }
                    }

                    MessageHandlerAttribute attribute = (MessageHandlerAttribute)attributes[0];
                    MessageHandler messageHandler = new MessageHandler(
                        FastMethodInvoker.GetMethodInvoker(handler, info),
                        handler,
                        attribute.PrimaryHandler,
                        needsChannelParameter, needsEndpointParameter);

                    messageHandlers[messageName].Remove(messageHandler);

                    if (messageHandlers[messageName].Count == 0)
                        messageHandlers.Remove(messageName);
                }
            }
        }

        //private static Guid currentConnectionID;
        //static public Guid CurrentConnectionID
        //{
        //    get { return currentConnectionID; }
        //}

        private readonly object syncRoot = new object();
        public virtual IMessage DispatchMessage(IMessage msg, ClientConnection channel)
        {
            return Dispatch(msg, channel);
        }

        public IMessage DispatchMessage(IMessage msg, IPEndPoint endpoint)
        {
            return Dispatch(msg, endpoint);
        }

        public IMessage DispatchMessage(IMessage msg)
        {
            return Dispatch(msg, null);
        }

        protected IMessage Dispatch(IMessage msg, object param)
        {
            lock (syncRoot)
            {
                //object[] parameters = new object[] { msg };
                if (!messageHandlers.ContainsKey(msg.Name))
                    throw new ProtocolException(msg.Name + " missing");

                List<MessageHandler> handlerList = messageHandlers[msg.Name];

                IMessage response = null;
                foreach (MessageHandler handler in handlerList)
                {
                    if (handler.IsPrimaryHandler)
                    {
                        try
                        {
                            response = handler.Execute(msg, param);
                        }
                           catch (ProtocolException ex)
                        {
                            // Convert older message handlers that throw ProtocolException into MessageProcessingExceptions
                            throw new ProtocolException(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            // Log and convert to MessageProcessingException
                            logger.Error("Error processing message " + msg.Name, ex);
                            throw new ProtocolException(ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            handler.Execute(msg, param);
                        }
                        catch (Exception)
                        {
                            logger.WarnFormat("Unhandled exception in non-primary message handler for message: {0} (ID:{1}).", msg.Name, msg.MessageID);
                        }
                    }
                }
                return response;
            }
        }

        public bool HasDispatcher(IMessage msg)
        {
            return messageHandlers.ContainsKey(msg.Name);
        }

    }

}

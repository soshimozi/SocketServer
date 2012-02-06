using System;
using System.Collections.Generic;
using SocketServer.Core.Messaging;
using SocketServer.Shared.Header;
using SocketServer.Reflection;
using SocketServer.Shared;
using SocketServer.Shared.Serialization;
using System.Reflection;
using log4net;
using SocketServer.Shared.Request;

namespace SocketServer.Command
{
    [Serializable]
    public class HandleClientRequestCommand : BaseMessageHandler
    {
        private readonly Guid _clientConnect;
        private readonly string _requestString;
        private readonly string _requestTypeString;
        private readonly string _handlerTypeString;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HandleClientRequestCommand(Guid connection, string handlerTypeString, string requestTypeString, string requestString)
        {
            _clientConnect = connection;
            _requestString = requestString;
            _requestTypeString = requestTypeString;
            _handlerTypeString = handlerTypeString;
        }

        public override void Execute()
        {
            object requestObject
                = DeSerializeRequest(
                    _requestTypeString,
                    _requestString);

            if (requestObject != null)
            {
                // find handler
                Type handlerType = ReflectionHelper.FindType(_handlerTypeString);

                if (handlerType != null)
                {
                    object handler = ReflectionHelper.ActivateObject(handlerType, null);

                    if (handler != null)
                    {
                        Type interfaceType = ReflectionHelper.FindGenericInterfaceMethod("IRequestHandler", new Type[] { typeof(NegotiateKeysRequest) }, handlerType);
                        if (interfaceType != null)
                        {
                            // we are in business
                            MethodInfo mi = interfaceType.GetMethod("HandleRequest");
                            try
                            {
                                mi.Invoke(handler, new object[] { requestObject, _clientConnect });
                            }
                            catch (Exception ex)
                            {
                                Logger.ErrorFormat("Error in HandleRequest\n. {0}", ex);
                            }
                        }
                    }

                }
                    
            }
        }

        private object DeSerializeRequest(string requestTypeString, string requestString)
        {
            Type requestType = ReflectionHelper.FindType(requestTypeString);
            if (requestType != null)
            {
                return XmlSerializationHelper.DeSerialize(requestString, requestType);
            }
            return null;
        }

    }
}

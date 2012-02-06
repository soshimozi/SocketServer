using System;
using System.Collections.Generic;
using SocketServer.Messaging;
using SocketServer.Shared.Header;
using SocketServer.Reflection;
using SocketServer.Shared;
using SocketServer.Shared.Serialization;
using System.Reflection;
using log4net;
using SocketServer.Shared.Request;
using SocketServer.Repository;

namespace SocketServer.Command
{
    [Serializable]
    public class HandleClientRequestCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _request;
        private readonly string _requestType;
        private readonly string _handlerName;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HandleClientRequestCommand(Guid id, string handlerName, string requestType, string request)
        {
            _clientId = id;
            _handlerName = handlerName;
            _requestType = requestType;
            _request = request;
        }

        public override void Execute()
        {
            object requestObject
                = DeSerializeRequest(
                    _requestType,
                    _request);

            if (requestObject != null)
            {
                ServiceHandlerLookup.Instance.InvokeHandler(_handlerName, requestObject, _clientId);

                //// find handler
                //Type handlerType = ReflectionHelper.FindType(_handlerTypeString);

                //if (handlerType != null)
                //{
                //    object handler = ReflectionHelper.ActivateObject(handlerType, null);

                //    if (handler != null)
                //    {
                //        Type interfaceType = ReflectionHelper.FindGenericInterfaceMethod("IRequestHandler", new Type[] { typeof(NegotiateKeysRequest) }, handlerType);
                //        if (interfaceType != null)
                //        {
                //            // we are in business
                //            MethodInfo mi = interfaceType.GetMethod("HandleRequest");
                //            try
                //            {
                //                mi.Invoke(handler, new object[] { requestObject, _clientConnect });
                //            }
                //            catch (Exception ex)
                //            {
                //                Logger.ErrorFormat("Error in HandleRequest\n. {0}", ex);
                //            }
                //        }
                //    }

                //}
                    
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

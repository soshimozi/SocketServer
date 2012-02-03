namespace SocketServer.Core.ServiceHandlerLib
{ 
    public interface IServiceHandler
    {
        bool HandleRequest(object request, object state);
    }
}

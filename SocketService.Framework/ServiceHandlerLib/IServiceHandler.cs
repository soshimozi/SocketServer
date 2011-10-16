namespace SocketService.Framework.ServiceHandlerLib
{ 
    public interface IServiceHandler
    {
        bool HandleRequest(object request, object state);
    }
}

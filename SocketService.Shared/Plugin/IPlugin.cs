namespace SocketService.Shared.Plugin
{
    public interface IPlugin
    {
        ChainAction UserSendPublicMessage(object context);
    }

    public enum ChainAction
    {
        Fail,
        Continue,
        Stop,
        NoAction
    }
}
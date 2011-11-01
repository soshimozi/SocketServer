namespace SocketService.Framework.Client.API
{
    public interface IPlugin
    {
        ChainAction UserSendPublicMessage(UserPublicMessageContext context);
    }

    public enum ChainAction
    {
        Fail,
        Continue,
        Stop,
        NoAction
    }
}
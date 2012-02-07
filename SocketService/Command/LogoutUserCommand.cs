using System;
using System.Linq;
using SocketServer.Actions;
using SocketServer.Messaging;
using SocketServer.Event;
using SocketServer.Net;
using SocketServer.Net.Client;
using SocketServer.Repository;

namespace SocketServer.Command
{
    [Serializable]
    internal class LogoutUserCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;

        public LogoutUserCommand(Guid clientId)
        {
            _clientId = clientId;
        }

        public override void Execute()
        {
            Logger.InfoFormat("Client {0} logging out.", _clientId);

            var connection = ConnectionRepository.Instance.Query( c => c.ClientId == _clientId).FirstOrDefault();
            if (connection != null)
                ConnectionRepository.Instance.RemoveConnection(connection);

            var clientSocket = SocketRepository.Instance.FindByClientId(_clientId);
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }

            var user = UserRepository.Instance.Query(u => u.ClientKey == _clientId).FirstOrDefault();
            if (user != null && user.Room != null)
            {
                //var userList = user.Room.Users.Select(u => u.ClientKey);
                //MSMQQueueWrapper.QueueCommand(
                //    new BroadcastMessageCommand<PublicMessageEvent>(userList.ToArray(),
                //                               new PublicMessageEvent
                //                                   {
                //                                       RoomId = (int) user.RoomId,
                //                                       UserName = string.Empty,
                //                                       Message = string.Format("{0} has logged out.", user.Name),
                //                                       ZoneId = (int) user.Room.ZoneId
                //                                   })
                //    );
            }

            UserActionEngine.Instance.LogoutUser(_clientId);
        }
    }
}
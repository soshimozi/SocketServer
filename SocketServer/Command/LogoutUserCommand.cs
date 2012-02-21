using System;
using System.Linq;
using SocketServer.Actions;
using SocketServer.Net;
using SocketServer.Net.Client;
using SocketServer.Repository;
using SocketServer.Shared.Network;
using log4net.Core;

namespace SocketServer.Command
{
    [Serializable]
    internal class LogoutUserCommand : BaseCommandHandler
    {
        private readonly ClientConnection connection;

        public LogoutUserCommand(ClientConnection connection)
        {
            this.connection = connection;
        }

        public override void Execute()
        {
            Logger.InfoFormat("Client {0} logging out.", connection.ClientId);

            foreach (var u in UserRepository.Instance.GetAll())
            {
                Logger.Logger.Log(typeof(LogoutUserCommand), Level.Finer, string.Format("name: {0}, id: {1}", u.Name, u.ClientKey), null);
            }

            var query = from u in UserRepository.Instance.GetAll()
                        where u.ClientKey.Equals(connection.ClientId)
                        select u;

            var user = query.FirstOrDefault();

            //// we have to log out user associated with this socket
            //var user 
            //    = UserRepository
            //    .Instance
            //    .Query(
            //        u => u.ClientKey.Equals(connection.ClientId)
            //        ).FirstOrDefault();

            if (user != null)
            {
                //if (user.Room != null)
                //{
                    //user.Room.Users.Remove(user);
                //}

                user.Room = null;
                UserRepository.Instance.Delete(user);

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

           // UserActionEngine.Instance.LogoutUser(connection.ClientId);
        }
    }
}
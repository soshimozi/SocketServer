using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Core
{
    public class MSMQSubscriber
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public MSMQSubscriber(string queueName, bool createIfNotExists)
        {
            // check if queue exists
            // if not, check flag createIfNotExists
            // if flag is set, create the queue, otherwise throw an exception

            Initialize();
        }

        private void Initialize()
        {
            // start poll receive thread
            // or call queue beginReceive

            // when a message is available
            // pull it and call listener
        }

        protected void OnMessageReceived(string message)
        {
            var callback = MessageReceived;
            if (callback != null)
            {
                callback(this, new MessageReceivedEventArgs(message));
            }
        }

    }
}

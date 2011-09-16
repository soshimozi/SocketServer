using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Configuration;

namespace CommandQueue
{
    public class MSMQQueueCommand
    {
        private static string _queuePath;

        static MSMQQueueCommand()
        {
            _queuePath = ConfigurationManager.AppSettings["ServerMessageQueue"];
        }

        public static void QueueCommand(ICommand c)
        {
            try
            {
                // open the queue
                MessageQueue mq = new MessageQueue(_queuePath);
                // set the message to durable.
                mq.DefaultPropertiesToSend.Recoverable = true;
                // set the formatter to Binary, default is XML
                mq.Formatter = new BinaryMessageFormatter();

                // send the command object
                mq.Send(c, "Command Message");
                mq.Close();
            }
            catch (Exception e)
            {
                // TODO: Log exception
            }
        }

    }
}

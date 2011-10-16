using System;
using System.Messaging;
using System.Configuration;
using log4net;
using System.Reflection;

namespace SocketService.Framework.Messaging
{
    public class MSMQQueueWrapper
    {
        private static readonly string QueuePath;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static MSMQQueueWrapper()
        {
            QueuePath = ConfigurationManager.AppSettings["ServerMessageQueue"];
        }

        /// <summary>
        /// Queues the command.
        /// </summary>
        /// <param name="c">The c.</param>
        public static void QueueCommand(ICommand c)
        {
            try
            {
                // open the queue
                var mq = new MessageQueue(QueuePath)
                 {DefaultPropertiesToSend = {Recoverable = true}, Formatter = new BinaryMessageFormatter()};

                // set the message to durable.

                // set the formatter to Binary, default is XML

                // send the command object
                mq.Send(c, "Command Message");
                mq.Close();
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error: {0}", e.Message);
            }
        }

    }
}

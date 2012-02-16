using System;
using System.Messaging;
using System.Configuration;
using log4net;
using System.Reflection;
using TestServer.Configuration;

namespace TestServer.Command
{
    public class MSMQQueueWrapper
    {
        private static readonly string QueuePath;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static MSMQQueueWrapper()
        {
            SocketServerConfiguration config = ServerConfigurationHelper.GetServerConfiguration();

            if (config.Queues.Count > 0)
            {
                QueuePath = config.Queues[0].QueuePath;
            }

            EnsureQueueExists(QueuePath);
        }

        private static void EnsureQueueExists(string path)
        {
            if (!MessageQueue.Exists(path))
            {
                MessageQueue.Create(path);
            }
        }

        /// <summary>
        /// Queues the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public static void QueueCommand(ICommand command)
        {
            try
            {
                // open the queue
                var mq = new MessageQueue(QueuePath)
                {
                     DefaultPropertiesToSend = {Recoverable = true}, 
                     Formatter = new BinaryMessageFormatter()
                };

                // set the message to durable.

                // set the formatter to Binary, default is XML

                // send the command object
                mq.Send(command);
                mq.Close();
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error: {0}", e.Message);
            }
        }

    }
}

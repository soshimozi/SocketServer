using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Configuration;
using log4net;
using System.Reflection;

namespace SocketService.Framework.Messaging
{
    public class MSMQQueueWrapper
    {
        private static string _queuePath;
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static MSMQQueueWrapper()
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
                Log.ErrorFormat("Error: {0}", e.Message);
            }
        }

    }
}

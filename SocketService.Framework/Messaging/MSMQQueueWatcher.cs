using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Diagnostics;
using log4net;
using System.Reflection;

namespace SocketService.Framework.Messaging
{
    public class MSMQQueueWatcher : IDisposable
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MessageQueue _receiveQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MSMQQueueWatcher"/> class.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        public MSMQQueueWatcher(string queueName)
        {
            // open the queue
            _receiveQueue = new MessageQueue(queueName);
            _receiveQueue.Formatter = new BinaryMessageFormatter();
        }

        /// <summary>
        /// Closes the queue.
        /// </summary>
        public void CloseQueue()
        {
            _receiveQueue.Close();
        }

        /// <summary>
        /// Recieves the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        public T RecieveMessage<T>(int milliseconds) where T : class
        {
            T t = default(T);
            try
            {
                Message myMessage = _receiveQueue.Receive(TimeSpan.FromMilliseconds(milliseconds));
                t = myMessage.Body as T;
            }
            catch (MessageQueueException e)
            {
                // if MessageQueue.Receive times out, we'll ignore the exception
                // otherwise, do something useful with the error (log, display, etc.)
                if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    Log.ErrorFormat("Error: ({0}) {1}", e.ErrorCode, e.Message);
                }
            }
            return t;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CloseQueue();
        }
    }
}

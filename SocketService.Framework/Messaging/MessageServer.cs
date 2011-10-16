using System;
using System.Messaging;
using System.Threading;
using System.Configuration;
using System.Reflection;
using log4net;

namespace SocketService.Framework.Messaging
{
    public class MessageServer : MSMQQueueWatcher
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);

        private bool _running;

        private readonly string _queuePath;
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer"/> class.
        /// </summary>
        public MessageServer() : base(ConfigurationManager.AppSettings["ServerMessageQueue"])
        {
            _queuePath = ConfigurationManager.AppSettings["ServerMessageQueue"];

            // create queue, if it doesn't exist
            if (!MessageQueue.Exists(_queuePath))
                MessageQueue.Create(_queuePath);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (_running) return;

            _stopEvent.Reset();
            _running = true;

            var numProcessors = Environment.ProcessorCount;
            const double numThreadsPerProcessor = 1.5;
            var numThreads = (int)(numProcessors * numThreadsPerProcessor) + 1;

            for (int i = 0; i < numThreads; i++)
            {
                var serverThread = new Thread(Serve);
                serverThread.Start();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _stopEvent.Set();
            _running = false;
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            // TODO: Implement Pause
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Resume()
        {
            // TODO: Implement Resume
        }

        protected void Serve()
        {
            while (!_stopEvent.WaitOne(50))
            {
                var command = RecieveMessage<ICommand>(500);
                if (command == null) continue;

                try
                {
                    command.Execute();
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Error: {0}", e.Message);
                }
            }
        }
    }
}

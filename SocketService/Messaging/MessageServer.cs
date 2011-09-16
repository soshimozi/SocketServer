using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Threading;
using System.Configuration;
using System.Reflection;
using log4net;

namespace SocketService.Messaging
{
    public class MessageServer : MSMQQueueWatcher
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _pauseEvent = new ManualResetEvent(false);

        private bool _running = false;

        private readonly string _queuePath;
        public MessageServer() : base(ConfigurationManager.AppSettings["ServerMessageQueue"])
        {
            _queuePath = ConfigurationManager.AppSettings["ServerMessageQueue"];

            // create queue, if it doesn't exist
            if (!MessageQueue.Exists(_queuePath))
                MessageQueue.Create(_queuePath);
        }

        public void Start()
        {
            if (!_running)
            {
                _stopEvent.Reset();
                _running = true;

                int numProcessors = System.Environment.ProcessorCount;
                double numThreadsPerProcessor = 1.5;
                int numThreads = (int)((double)numProcessors * numThreadsPerProcessor) + 1;

                for (int i = 0; i < numThreads; i++)
                {
                    Thread serverThread = new Thread(new ThreadStart(Serve));
                    serverThread.Start();
                }
            }
        }

        public void Stop()
        {
            _stopEvent.Set();
            _running = false;
        }

        public void Pause()
        {
            // TODO: Implement Pause
        }

        public void Resume()
        {
            // TODO: Implement Resume
        }

        protected void Serve()
        {
            while (!_stopEvent.WaitOne(50))
            {
                ICommand command = RecieveMessage<ICommand>(500);
                if (command != null)
                {
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
}

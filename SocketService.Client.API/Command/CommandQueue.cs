using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketService.Client.API.Command
{
    class CommandQueue
    {
        private long _running;

        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private object _queueLock = new object();
        private Queue<ICommand> _commandQueue = new Queue<ICommand>();

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (Interlocked.Read(ref _running) == 0)
            {
                _stopEvent.Reset();
                Interlocked.Exchange(ref _running, 1);

                int numProcessors = System.Environment.ProcessorCount;
                double numThreadsPerProcessor = 1.5;
                int numThreads = (int)((double)numProcessors * numThreadsPerProcessor) + 1;

                for (int i = 0; i < numThreads; i++)
                {
                    Thread serverThread = new Thread(new ThreadStart(CommandThread));
                    serverThread.Start();
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _stopEvent.Set();
            Interlocked.Exchange(ref _running, 0);
        }

        protected virtual void CommandThread()
        {
            while (_stopEvent.WaitOne(100))
            {
                ICommand command = NextCommand();
                if (command != null)
                {
                    // execute the command
                    command.Execute();
                }
            }
        }

        private ICommand NextCommand()
        {
            lock (_queueLock)
            {
                if (_commandQueue.Count > 0)
                {
                    return _commandQueue.Dequeue();
                }
            }

            return null;
        }
    
    }
}

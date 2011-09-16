using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace CommandQueue
{
    public class ObjectQueue
    {
        private static int MAX_REQUEST_TIME = 45000;
        private ManualResetEvent _stopEvent;
        private Mutex _queueLock = new Mutex();
        private Mutex _timestampLock = new Mutex();

        private volatile bool _isRunning = false;

        private Queue<ICommand> _commandQueue = new Queue<ICommand>();
 

        private int _threadCount;

        Thread[] _threads;
        private DateTime[] _threadTimeStamps;

        protected static ILog log = LogManager.GetLogger(typeof(ObjectQueue));


        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectQueue"/> class.
        /// </summary>
        /// <param name="threadCount">The thread count.</param>
        public ObjectQueue(int threadCount)
        {
            _threadCount = threadCount;

            _stopEvent = new ManualResetEvent(false);

            registerThreads(threadCount);

#if !DISABLE_WATCHDOG
            startWatchdogThread();
#endif
        }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void AddCommand(ICommand command)
        {
            // get queue lock to insert command object
            _queueLock.WaitOne();

            try
            {
                _commandQueue.Enqueue(command);
            }
            finally
            {
                _queueLock.ReleaseMutex();
            }

        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _stopEvent.Set();
        }

        private void registerThreads(int threadCount)
        {
            _threads = new Thread[threadCount];
            _threadTimeStamps = new DateTime[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ThreadFunction));
                _threads[i] = thread;
            }

        }

        public void Start()
        {
            if (!_isRunning)
            {
                _stopEvent.Reset();
                for (int index = 0; index < _threadCount; index++)
                {
                    _threads[index].Start(index);
                }

                _isRunning = true;
            }
        }

        private void startWatchdogThread()
        {
            Thread watchdog = new Thread(
                new ThreadStart(
                    () =>
                    {
                        while (!_stopEvent.WaitOne(500))
                        {
                            for (int index = 0; index < _threadCount; index++)
                            {
                                _timestampLock.WaitOne();

                                DateTime timeStamp = DateTime.MaxValue;
                                try
                                {
                                    timeStamp = _threadTimeStamps[index];
                                }
                                finally
                                {
                                    _timestampLock.ReleaseMutex();
                                }

                                TimeSpan timeDiff = (DateTime.Now - timeStamp);

                                if (timeStamp != DateTime.MaxValue && timeStamp != DateTime.MinValue && timeDiff.TotalMilliseconds > MAX_REQUEST_TIME)
                                {
                                    log.Debug(string.Format("Terminating slow thread: {0}, time taken: {1}.", index, timeDiff.ToString()));
                                    _threads[index].Abort();

                                    _threads[index] = new Thread(new ParameterizedThreadStart(ThreadFunction));
                                    // restart to continue listening
                                    _threads[index].Start();

                                    log.Debug("Thread restarted.");
                                }
                            }

                        }
                    }
                )
            );

            watchdog.Start();

        }


        private void ThreadFunction(object param)
        {
            int index = 0;

            if( param != null )
            {
                index = (int)param;    
            }

            while (!_stopEvent.WaitOne(10))
            {
                // then get a hold of the queue, this is needed to syncrhonize object extraction and insertion!
                _queueLock.WaitOne();

                ICommand command = null;

                try
                {
                    if (_commandQueue.Count > 0)
                    {
                        command = _commandQueue.Dequeue();
                    }
                }
                finally
                {
                    // Release the queue
                    _queueLock.ReleaseMutex();
                }

                if (command != null)
                {
                    _timestampLock.WaitOne();

                    try
                    {
                        _threadTimeStamps[index] = DateTime.Now;
                    }
                    finally
                    {
                        _timestampLock.ReleaseMutex();
                    }

                    log.Info(string.Format("Executing command for worker slot {0}", index));

                    // execute the command object
                    command.Execute();

                    _timestampLock.WaitOne();

                    try
                    {
                        _threadTimeStamps[index] = DateTime.MaxValue;
                    }
                    finally
                    {
                        _timestampLock.ReleaseMutex();
                    }
                }
            }
        }
    }
}

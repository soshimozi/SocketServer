using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketService.Commands
{
    public class CommandQueue
	{
        //#region CommandWrapper class
        //private class CommandWrapper
        //{
        //    public Action<T> Action;
        //    public T Value;
        //}
        //#endregion

		#region Private Instance Fields
		private ManualResetEvent _stopEvent;
        private Mutex _queueLock = new Mutex();
        private Mutex _threadPoolLock = new Mutex();
        private Queue<ICommand> _commandQueue = new Queue<ICommand>();
        private CommandThread[] _threadPool;
        private bool _isRunning;
		#endregion

		#region Public Instance Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandQueue&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="numThreads">The num threads.</param>
		public CommandQueue(int numThreads)
        {
            _stopEvent = new ManualResetEvent(false);

            _isRunning = false;
            CreateThreadPool(numThreads);
        }

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			if (!IsRunning)
			{
				_stopEvent.Reset();

				IsRunning = true;
				StartThreadPool();
			}
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				_stopEvent.Set();
				IsRunning = false;
			}
		}

		/// <summary>
		/// Adds the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="value">The value.</param>
		public void AddCommand(ICommand command)
		{
			_queueLock.WaitOne();
			try
			{
				_commandQueue.Enqueue(command);
			}
			finally
			{
				// Release the queue
				_queueLock.ReleaseMutex();
			}
		}
		#endregion

		#region Public Instance Properties
		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
		/// </value>
        public bool IsRunning
        {
            get
            {
                Thread.MemoryBarrier();
                return _isRunning;
            }

            private set
            {
                _isRunning = value;
                Thread.MemoryBarrier();
            }

        }
		#endregion

		#region Private Instance Methods
		private void StartThreadPool()
        {
            for (int i = 0; i < _threadPool.Length; i++)
            {
                _threadPool[i].Start(i);
            }
        }

        private void CreateThreadPool(int numThreads)
        {
            _threadPool = new CommandThread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                _threadPool[i] = new CommandThread(ThreadFunction);
            }
        }

        private void ThreadFunction(object param)
        {
            int index = 0;
            if (param != null)
            {
                index = (int)param;
            }

            while (!_stopEvent.WaitOne(20))
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
                    _threadPoolLock.WaitOne();

                    try
                    {
                        _threadPool[index].LastHeartBeat = DateTime.Now;
                    }
                    finally
                    {
                        _threadPoolLock.ReleaseMutex();
                    }

                    //log.Info(string.Format("Executing command for worker slot {0}", index));

                    // execute the command object
                    command.Action.BeginInvoke(command.Value,
                        new AsyncCallback(
                            (ar) =>
                                {
                                    if (ar.IsCompleted)
                                    {
                                        var wrapper = ar.AsyncState as CommandWrapper;
                                        if (wrapper != null)
                                        {
                                            try
                                            {
                                                wrapper.Action.EndInvoke(ar);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }), command);

                    _threadPoolLock.WaitOne();

                    try
                    {
                        // reset last heart beat
                        _threadPool[index].LastHeartBeat = DateTime.MaxValue;
                    }
                    finally
                    {
                        _threadPoolLock.ReleaseMutex();
                    }
                }
            }
		}
		#endregion
	}
}

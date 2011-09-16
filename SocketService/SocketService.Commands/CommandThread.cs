using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketService.Commands
{
    public class CommandThread
    {
        private bool _isStarted = false;
        private Thread _thread;

        public DateTime LastHeartBeat
        {
            get;
            set;
        }

        public CommandThread(ThreadStart start)
        {
            _thread = new Thread(start);
        }

        public CommandThread(ParameterizedThreadStart parametizedStart)
        {
            _thread = new Thread(parametizedStart);
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;
                _thread.Start();
            }
        }

        public void Start(object parameter)
        {
            if (!_isStarted)
            {
                _isStarted = true;
                _thread.Start(parameter);
            }
        }
    }
}

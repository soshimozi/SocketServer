using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Client
{
    public class CommunicationErrorArgs : EventArgs
    {
        public CommunicationErrorArgs()
        {

        }

        public CommunicationErrorArgs(CommuncationErrorType errorType)
        {
            ErrorType = errorType;
        }

        public CommuncationErrorType ErrorType
        {
            get;
            set;
        }
    }

    public enum CommuncationErrorType
    {
        ReadTimeout,
        SendTimeout
    }

}

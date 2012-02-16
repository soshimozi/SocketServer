using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MessageHandlerAttribute : Attribute
    {

        // This value indicates whether the method using this attribute is the primary
        // message handler.  Primary message handlers are allowed to respond to messages.
        // Non-primary handlers are only allowed to read messages.
        private bool isPrimary = true;
        public bool PrimaryHandler
        {
            get { return isPrimary; }
            set { isPrimary = value; }
        }
    }
}

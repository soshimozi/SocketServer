using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Response
{
    [Serializable]
    public class ServerMessage
    {
        private readonly string _message;
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServerMessage(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessage"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public ServerMessage(string format, params object[] args)
        {
            _message = string.Format(format, args);
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}

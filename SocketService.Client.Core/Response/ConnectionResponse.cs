using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Response
{
    public class ConnectionResponse : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConnectionResponse"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get;
            set;
        }
    }
}

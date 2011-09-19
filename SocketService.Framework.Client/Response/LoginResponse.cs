using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LoginResponse"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
    
    }
}

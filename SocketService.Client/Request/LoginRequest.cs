using System;

namespace SocketService.Client.Core.Request
{
    [Serializable]
    public class LoginRequest 
    {
        /// <summary>
        /// Gets or sets the name of the login.
        /// </summary>
        /// <value>
        /// The name of the login.
        /// </value>
        public string LoginName
        {
            get;
            set;
        }
    }
}

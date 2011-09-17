using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Crypto;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class GetCentralAuthorityRequest 
    {
        /// <summary>
        /// Gets or sets the central authority.
        /// </summary>
        /// <value>
        /// The central authority.
        /// </value>
        public CentralAuthority CentralAuthority
        {
            get;
            set;
        }
    }
}

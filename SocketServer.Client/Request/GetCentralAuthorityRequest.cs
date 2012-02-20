using System;
using SocketService.Core.Crypto;

namespace SocketService.Client.Core.Request
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
        public CentralAuthority CentralAuthority { get; set; }
    }
}
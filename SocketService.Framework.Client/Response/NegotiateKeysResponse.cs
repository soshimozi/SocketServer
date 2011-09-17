using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class NegotiateKeysResponse
    {
        /// <summary>
        /// Gets or sets the remote public key.
        /// </summary>
        /// <value>
        /// The remote public key.
        /// </value>
        public byte[] RemotePublicKey
        {
            get;
            set;
        }

    }
}

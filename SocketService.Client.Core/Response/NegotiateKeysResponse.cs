using System;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class NegotiateKeysResponse : IResponse
    {
        /// <summary>
        /// Gets or sets the remote public key.
        /// </summary>
        /// <value>
        /// The remote public key.
        /// </value>
        public byte[] RemotePublicKey { get; set; }
    }
}
using System;

namespace SocketServer.Shared.Response
{
    [Serializable]
    public class NegotiateKeysResponse : IServerResponse
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

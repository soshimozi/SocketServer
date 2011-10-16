using System;

namespace SocketService.Framework.Client.Request
{
    [Serializable]
    public class NegotiateKeysRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NegotiateKeysRequest"/> class.
        /// </summary>
        public NegotiateKeysRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NegotiateKeysRequest"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public NegotiateKeysRequest(byte[] key)
        {
            PublicKey = key;
        }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        /// <value>
        /// The public key.
        /// </value>
        public byte[] PublicKey { get; set; }
    }
}
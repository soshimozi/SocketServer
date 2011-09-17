using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Request
{
    [Serializable()]
    public class ClientRequestHeader : IRequestHeader
    {
        private readonly byte[] _publicKey;
        private readonly EncryptionType _encryption;
        private readonly DateTime _timestamp;
        private readonly uint _crc32;
        private readonly byte [] _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRequestHeader"/> class.
        /// </summary>
        /// <param name="EncryptionPublicKey">The encryption public key.</param>
        /// <param name="Encryption">The encryption.</param>
        /// <param name="TimeStamp">The time stamp.</param>
        /// <param name="CRC32">The CR C32.</param>
        /// <param name="Request">The request.</param>
        public ClientRequestHeader(byte[] EncryptionPublicKey,
                EncryptionType Encryption, DateTime TimeStamp,
                uint CRC32, byte [] Request)
        {
            _publicKey = EncryptionPublicKey;
            _encryption = Encryption;
            _timestamp = TimeStamp;
            _crc32 = CRC32;
            _request = Request;
        }

        /// <summary>
        /// Gets the encryption public key.
        /// </summary>
        public byte[] EncryptionPublicKey
        {
            get { return _publicKey; }
        }

        /// <summary>
        /// Gets the encryption.
        /// </summary>
        public EncryptionType Encryption
        {
            get { return _encryption; }
        }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// Gets the CR C32.
        /// </summary>
        public uint CRC32
        {
            get { return _crc32; }
        }

        /// <summary>
        /// Gets the request data.
        /// </summary>
        public byte[] RequestData
        {
            get { return _request; }
        }
    }
}

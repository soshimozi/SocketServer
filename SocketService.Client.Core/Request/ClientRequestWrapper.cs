using System;

namespace SocketService.Client.Core.Request
{
    [Serializable]
    public class ClientRequestWrapper //: IRequest
    {
        private readonly byte[] _publicKey;
        private readonly EncryptionType _encryption;
        private readonly DateTime _timestamp;
        private readonly uint _crc32;
        private readonly byte [] _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRequestWrapper"/> class.
        /// </summary>
        /// <param name="encryptionPublicKey">The encryption public key.</param>
        /// <param name="encryption">The encryption.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="request">The request.</param>
        public ClientRequestWrapper(byte[] encryptionPublicKey,
                                    EncryptionType encryption, 
                                    DateTime timeStamp, 
                                    byte [] request) 
            : this(encryptionPublicKey, encryption, timeStamp, 0, request)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRequestWrapper"/> class.
        /// </summary>
        /// <param name="encryptionPublicKey">The encryption public key.</param>
        /// <param name="encryption">The encryption.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="crc32">The CR C32.</param>
        /// <param name="request">The request.</param>
        public ClientRequestWrapper(byte[] encryptionPublicKey,
                EncryptionType encryption, DateTime timeStamp,
                uint crc32, byte [] request)
        {
            _publicKey = encryptionPublicKey;
            _encryption = encryption;
            _timestamp = timeStamp;
            _crc32 = crc32;
            _request = request;
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

    public enum EncryptionType
    {

        /// <remarks/>
        AES,

        /// <remarks/>
        DES,

        /// <remarks/>
        TripleDES,

        /// <remarks/>
        Rijindal,

        None,
    }
}

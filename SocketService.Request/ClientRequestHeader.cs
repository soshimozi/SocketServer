using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Request
{
    [Serializable()]
    public class ClientRequestHeader : IRequestHeader
    {
        private readonly byte[] _publicKey;
        private readonly EncryptionType _encryption;
        private readonly DateTime _timestamp;
        private readonly uint _crc32;
        private readonly byte [] _request;

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

        public byte[] EncryptionPublicKey
        {
            get { return _publicKey; }
        }

        public EncryptionType Encryption
        {
            get { return _encryption; }
        }

        public DateTime TimeStamp
        {
            get { return _timestamp; }
        }

        public uint CRC32
        {
            get { return _crc32; }
        }

        public byte[] RequestData
        {
            get { return _request; }
        }
    }
}

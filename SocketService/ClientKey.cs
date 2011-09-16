using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Crypto;
using System.Security.Cryptography;

namespace SocketService
{
    public class ClientKey : DHKeyBase
    {
        private readonly Guid _clientId;

        public ClientKey(Guid clientId)
            : base()
        {
            this._clientId = clientId;
        }

        public Guid ClientId
        {
            get { return _clientId; }
        }

        public byte[] Exchange(byte[] remoteKey)
        {
            CngKey cngKey = CngKey.Import(remoteKey, CngKeyBlobFormat.EccPublicBlob);
            return dh.DeriveKeyMaterial(cngKey);
        }

        public byte[] PublicId 
        {
            get { return dh.PublicKey.ToByteArray(); } 
        }
    }
}

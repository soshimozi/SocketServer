using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SocketService.Crypto
{
    public class DHKeyPair : DHKeyBase
    {
        public DHKeyPair() : base()
        {
            PublicKey = dh.PublicKey.ToByteArray();
        }

        public byte[] PublicKey
        {
            get;
            private set;
        }

        //public byte[] PrivateKey
        //{
        //    get;
        //    private set;
        //}

        public byte[] Exchange(byte[] remotePublicKey)
        {
            CngKey cngKey = CngKey.Import(remotePublicKey, CngKeyBlobFormat.EccPublicBlob);
            return dh.DeriveKeyMaterial(cngKey);
        }
    }
}

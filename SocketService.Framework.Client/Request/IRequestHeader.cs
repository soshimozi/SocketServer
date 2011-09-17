using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Request
{
    public interface IRequestHeader
    {
        byte[] EncryptionPublicKey
        {
            get;
        }

        EncryptionType Encryption
        {
            get;
        }

        DateTime TimeStamp
        {
            get;
        }

        uint CRC32
        {
            get;
        }

        byte [] RequestData
        {
            get;
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

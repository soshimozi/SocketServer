using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Crypto
{
    public interface ICrypto
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        byte[] Encrypt(byte[] plainBytes);
        byte[] Decrypt(byte[] cipherBytes);
    }
}

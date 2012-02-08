using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Header;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;

namespace SocketServer.Shared
{
    public static class RequestBuilder
    {
        public static RequestHeader BuildRequestHeader(EncryptionTypes encryptionType, CompressionTypes compressionType, RequestTypes requestType)
        {
            // first create headers
            EncryptionHeader encryptionHeader = new EncryptionHeader()
            {
                EncryptionType = encryptionType
            };

            MessageHeader messageHeader = new MessageHeader()
            {
                CompressionType = compressionType,
                EncryptionHeader = encryptionHeader
            };

            RequestHeader requestHeader = new RequestHeader()
            {
                RequestType = requestType,
                MessageHeader = messageHeader
            };

            // send response header first
            return requestHeader;

        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="publicKeyEncoded">The public key encoded.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static String ProcessRequest(ServerAuthority serverAuthority, byte[] publicKeyEncoded, RequestHeader header, String request)
        {
            if (header.MessageHeader.EncryptionHeader.EncryptionType != EncryptionTypes.None)
            {
                //byte[] publicKeyEncoded = client.RequestHeader.MessageHeader.EncryptionHeader.PublicKey;
                DHPublicKeyParameters publicKey = new DHPublicKeyParameters(
                    ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(publicKeyEncoded)).Y, serverAuthority.Parameters);

                BigInteger agreementValue = serverAuthority.GenerateAgreementValue(publicKey);

                RijndaelCrypto crypto = new RijndaelCrypto();
                return crypto.Encrypt(request, agreementValue.ToString(16));
            }
            else
            {
                return request;
            }
        }
    }
}

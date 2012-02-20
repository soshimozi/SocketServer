using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Header;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using SocketServer.Crypto;

namespace SocketServer.Shared
{
    public static class ResponseBuilder
    {
        public static ResponseHeader BuildResponseHeader(EncryptionTypes encryptionType, CompressionTypes compressionType, ResponseTypes responseType)
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

            ResponseHeader responseHeader = new ResponseHeader()
            {
                ResponseType = responseType,
                MessageHeader = messageHeader
            };

            // send response header first
            return responseHeader;

        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="publicKeyEncoded">The public key encoded.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static String ProcessResponse(ServerAuthority serverAuthority, byte[] publicKeyEncoded, ResponseHeader header, String response)
        {
            if (header.MessageHeader.EncryptionHeader.EncryptionType != EncryptionTypes.None)
            {
                //byte[] publicKeyEncoded = client.RequestHeader.MessageHeader.EncryptionHeader.PublicKey;
                DHPublicKeyParameters publicKey = new DHPublicKeyParameters(
                    ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(publicKeyEncoded)).Y, serverAuthority.Parameters);

                BigInteger agreementValue = serverAuthority.GenerateAgreementValue(publicKey);

                RijndaelCrypto crypto = new RijndaelCrypto();
                return crypto.Encrypt(response, agreementValue.ToString(16));
            }
            else
            {
                return response;
            }
        }
    }
}

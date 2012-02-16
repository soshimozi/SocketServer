using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Messages;
using SocketServer.Shared.Network;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using SocketServer.Crypto;
using SocketServer.Shared.Messaging;

namespace SocketServer.Handler
{
    public class EnableEncryptionHandler : IRequestHandler<EnableEncryptionMessage>
    {
        public void HandleRequest(EnableEncryptionMessage request, ClientConnection connection)
        {

            connection.Send(
                new EnableEncryptionResponse()
                {
                    MessageID = "EnableEncryptionResponse",
                    Enabled = true
                });

            // replace   0 envelope with a new encrypted message envelope
            connection.Envelope = new EncryptedMessageEnvelope(new SharedKeyCrypto(connection.ServerAuthority.Parameters, connection.ServerAuthority.KeyPair, request.PublicKeyInfo));

        }
    }
}

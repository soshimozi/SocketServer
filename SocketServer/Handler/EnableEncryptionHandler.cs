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
using com.BlazeServer.Messages.MessageProtos;
using Google.ProtocolBuffers;

namespace SocketServer.Handler
{
    public class EnableEncryptionHandler : IRequestHandler<EnableEncryptionRequest>
    {
        public void HandleRequest(EnableEncryptionRequest request, ClientConnection connection)
        {
            EnableEncryptionResponse.Builder newResponse =
                EnableEncryptionResponse.CreateBuilder();

            newResponse
                .SetMessageId(1)
                .SetSuccess(true);

            newResponse.SetPublickey(
                ByteString.CopyFrom(
                    connection
                    .ServerAuthority
                    .GenerateEncodedPublicKeyInfo()));

            EnableEncryptionResponse response = newResponse.Build();
            connection.Send(response);

            if (request.Enable)
            {
                ((ProtoBuffEnvelope)connection.Envelope)
                .EnableEncryption(
                    connection.ServerAuthority,
                    request.Publickey.ToArray());

            }
            else
            {
                ((ProtoBuffEnvelope)connection.Envelope).DisableEncryption();
            }

        }
    }
}

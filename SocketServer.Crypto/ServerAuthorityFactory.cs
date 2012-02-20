using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Crypto
{
    public static class ServerAuthorityFactory
    {
        public static ServerAuthority CreateServerAuthority()
        {
            return new ServerAuthority(Constants.DefaultDiffieHellmanKeyLength, Constants.DefaultPrimeProbability);
        }
    }
}

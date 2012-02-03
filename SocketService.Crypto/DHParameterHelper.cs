using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace SocketServer.Crypto
{
    public static class DHParameterHelper
    {
        public static DHParameters GenerateParameters(byte [] pdata, byte [] gdata)
        {
            return new DHParameters(new BigInteger(pdata), new BigInteger(gdata));
        }

        public static DHParameters GenerateParameters(int keyLength, int probability)
        {
            DHParametersGenerator generator = new DHParametersGenerator();
            generator.Init(keyLength, probability, new SecureRandom());
            return generator.GenerateParameters();
        }

        public static DHParameters GenerateParameters()
        {
            return GenerateParameters(SocketServer.Crypto.Constants.DefaultDiffieHellmanKeyLength, SocketServer.Crypto.Constants.DefaultPrimeProbability);
        }

    }
}

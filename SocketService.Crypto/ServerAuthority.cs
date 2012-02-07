using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;

namespace SocketServer.Crypto
{
    public class ServerAuthority
    {
        //const int BitLength = 512;
        //const int Probability = 30;

        private readonly DHParameters parameters;
        private readonly AsymmetricCipherKeyPair kp;
        private readonly IBasicAgreement agreement;

        public ServerAuthority(int bitLength, int probability)
        {
            parameters = GetDiffieHellmanParameters(bitLength, probability);

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), parameters);

            keyGen.Init(kgp);
            kp = keyGen.GenerateKeyPair();
            agreement = AgreementUtilities.GetBasicAgreement("DH");
            agreement.Init(kp.Private);
        }

        public DHParameters Parameters
        {
            get { return parameters;  }
        }
    

        public BigInteger G
        {
            get { return parameters.G; }
        }

        public BigInteger P
        {
            get { return parameters.P; }
        }

        private DHParameters GetDiffieHellmanParameters(int bitLength, int probability)
        {
            DHParametersGenerator generator = new DHParametersGenerator();
            generator.Init(bitLength, probability, new SecureRandom());
            return generator.GenerateParameters();
        }

        //public byte[] GetEncryptedPublicKey()
        //{
        //    return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public).GetDerEncoded();
        //}

        public AsymmetricKeyParameter GetPublicKeyParameter()
        {
            return kp.Public;
        }

        public BigInteger GenerateAgreementValue(AsymmetricKeyParameter remotePublicKey)
        {
            return agreement.CalculateAgreement(remotePublicKey);
        }

    }
}

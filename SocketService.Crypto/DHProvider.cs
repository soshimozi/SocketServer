using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;

namespace SocketServer.Crypto
{
    public class DHProvider
    {
        private readonly DHParameters _parameters;
        private readonly AsymmetricCipherKeyPair _kp;
        private BigInteger _agreement;

        public DHProvider(DHParameters parameters)
        {
            _parameters = parameters;

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), _parameters);

            keyGen.Init(kgp);
            _kp = keyGen.GenerateKeyPair();
        }

        public DHProvider(int bitLength, int probability)
            : this(DHParameterHelper.GenerateParameters(bitLength, probability))
        {
        }

        public DHProvider()
            : this(DHParameterHelper.GenerateParameters())
        {
        }

        public AsymmetricKeyParameter RemotePublicKey
        {
            get;
            set;
        }

        public DHParameters Parameters
        {
            get { return _parameters;  }
        }

        //private static DHParameters GetDiffieHellmanParameters(int bitLength, int probability)
        //{
        //    DHParametersGenerator generator = new DHParametersGenerator();
        //    generator.Init(bitLength, probability, new SecureRandom());
        //    return generator.GenerateParameters();
        //}

        public byte[] GetEncryptedPublicKey()
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_kp.Public);
            return publicKeyInfo.ToAsn1Object().GetDerEncoded();
        }

        public byte [] Agree()
        {
            IBasicAgreement iba;
            iba = AgreementUtilities.GetBasicAgreement("DH");
            iba.Init(_kp.Private);

            _agreement = iba.CalculateAgreement(RemotePublicKey);
            return _agreement.ToByteArray();
        }
    }
}

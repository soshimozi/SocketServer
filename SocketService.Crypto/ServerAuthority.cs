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
using Org.BouncyCastle.Asn1.X509;

namespace SocketServer.Crypto
{
    public class ServerAuthority
    {
        private readonly DHParameters parameters;
        private readonly AsymmetricCipherKeyPair kp;
        private readonly IBasicAgreement agreement;

        public ServerAuthority(BigInteger prime, BigInteger g) 
            : this(new DHParameters(prime, g))
        {
        }

        public ServerAuthority(int bitLength, int probability)
            : this(GetDiffieHellmanParameters(bitLength, probability))
        {
        }

        public ServerAuthority(DHParameters parameters)
        {
            this.parameters = parameters;

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), parameters);

            keyGen.Init(kgp);
            kp = keyGen.GenerateKeyPair();
            agreement = AgreementUtilities.GetBasicAgreement("DH");
            agreement.Init(kp.Private);
        }


        public AsymmetricCipherKeyPair KeyPair
        {
            get { return kp;  }
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

        private static DHParameters GetDiffieHellmanParameters(int bitLength, int probability)
        {
            DHParametersGenerator generator = new DHParametersGenerator();
            generator.Init(bitLength, probability, new SecureRandom());
            return generator.GenerateParameters();
        }

        public AsymmetricKeyParameter GetPublicKeyParameter()
        {
            return kp.Public;
        }

        public byte [] GenerateEncodedPublicKeyInfo()
        {
            SubjectPublicKeyInfo publicKeyInfo 
                = SubjectPublicKeyInfoFactory
                    .CreateSubjectPublicKeyInfo(GetPublicKeyParameter());

            return publicKeyInfo.ToAsn1Object().GetDerEncoded();
        }

        public BigInteger GenerateAgreementValue(AsymmetricKeyParameter remotePublicKey)
        {
            return agreement.CalculateAgreement(remotePublicKey);
        }

    }
}

package com.BlazeServer.Crypto;

import java.math.BigInteger;
import java.security.SecureRandom;

import org.bouncycastle.asn1.DEREncodable;
import org.bouncycastle.asn1.DERInteger;
import org.bouncycastle.asn1.pkcs.DHParameter;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.asn1.x509.SubjectPublicKeyInfo;
import org.bouncycastle.crypto.AsymmetricCipherKeyPair;
import org.bouncycastle.crypto.KeyGenerationParameters;
import org.bouncycastle.crypto.agreement.DHBasicAgreement;
import org.bouncycastle.crypto.generators.DHBasicKeyPairGenerator;
import org.bouncycastle.crypto.generators.DHParametersGenerator;
import org.bouncycastle.crypto.params.AsymmetricKeyParameter;
import org.bouncycastle.crypto.params.DHKeyGenerationParameters;
import org.bouncycastle.crypto.params.DHParameters;
import org.bouncycastle.crypto.params.DHPublicKeyParameters;

public class ServerAuthority {

	private final DHParameters parameters;
	private final AsymmetricCipherKeyPair keyPair;
	private final DHBasicAgreement agreement;
	
	public ServerAuthority(BigInteger prime, BigInteger g)  {
		this(new DHParameters(prime, g));
	}
	
	public ServerAuthority(int bitLength, int primeProbability) {
		this(getDiffieHellmanParameters(bitLength, primeProbability));
	}

	public ServerAuthority(DHParameters parameters) {
		this.parameters = parameters;
		
        DHBasicKeyPairGenerator keyPairGenerator = new DHBasicKeyPairGenerator();
        KeyGenerationParameters keyGenerationParameters = new DHKeyGenerationParameters(new SecureRandom(), parameters);
        keyPairGenerator.init(keyGenerationParameters);
        
        this.keyPair = keyPairGenerator.generateKeyPair();
        this.agreement = new DHBasicAgreement();
        this.agreement.init(keyPair.getPrivate());
	}
	
	public DHParameters getParameters() {
		return this.parameters;
	}
	
	public AsymmetricCipherKeyPair getKeyPair() {
		return this.keyPair;
	}
	
	public BigInteger getP() {
		return this.parameters.getP();
	}
	
	public BigInteger getG() {
		return this.parameters.getG();
	}
	
	public AsymmetricKeyParameter getPublicKeyParameter() {
		return (AsymmetricKeyParameter) this.keyPair.getPublic();
	}
	
    public byte [] generateEncodedPublicKeyInfo()
    {
        DHPublicKeyParameters key = (DHPublicKeyParameters) keyPair.getPublic();
		DHParameters kp = key.getParameters();
    	
		SubjectPublicKeyInfo keyInfo = new SubjectPublicKeyInfo(
            new AlgorithmIdentifier(
				PKCSObjectIdentifiers.dhKeyAgreement,
				new DHParameter(kp.getP(), kp.getG(), kp.getL()).toASN1Object()),
				(DEREncodable) new DERInteger(key.getY()));   

        return keyInfo.toASN1Object().getDEREncoded();
    }

    public BigInteger generateAgreementValue(AsymmetricKeyParameter remotePublicKey)
    {
        return agreement.calculateAgreement(remotePublicKey);
    }	
	
    private static DHParameters getDiffieHellmanParameters(int bitLength, int probability)
    {
        DHParametersGenerator generator = new DHParametersGenerator();
        generator.init(bitLength, probability, new SecureRandom());
        return generator.generateParameters();
    }	
	
}

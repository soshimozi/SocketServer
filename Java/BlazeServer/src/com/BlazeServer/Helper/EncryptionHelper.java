package com.BlazeServer.Helper;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.security.GeneralSecurityException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyException;
import java.security.NoSuchAlgorithmException;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import org.apache.commons.codec.binary.Base64;
import org.bouncycastle.asn1.DEREncodable;
import org.bouncycastle.asn1.DERInteger;
import org.bouncycastle.asn1.pkcs.DHParameter;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.asn1.x509.SubjectPublicKeyInfo;
import org.bouncycastle.crypto.params.DHParameters;
import org.bouncycastle.crypto.params.DHPublicKeyParameters;

public class EncryptionHelper {
	private final static String characterEncoding = "UTF-8";
	private final static String cipherTransformation = "AES/CBC/PKCS5Padding";
	private final static String aesEncryptionAlgorithm = "AES";

	public static byte [] encodeKeyParameter(DHPublicKeyParameters key) {
		
		DHParameters keyParameters = key.getParameters();
		
		SubjectPublicKeyInfo keyInfo = new SubjectPublicKeyInfo(
            new AlgorithmIdentifier(
				PKCSObjectIdentifiers.dhKeyAgreement,
				new DHParameter(keyParameters.getP(), keyParameters.getG(), keyParameters.getL()).toASN1Object()),
				(DEREncodable) new DERInteger(key.getY()));
		
		
		return keyInfo.toASN1Object().getDEREncoded();
	}
	
	public static byte[] decrypt(byte[] cipherText, byte[] key, byte [] initialVector) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException
	{
		Cipher cipher = Cipher.getInstance(cipherTransformation);
		SecretKeySpec secretKeySpecy = new SecretKeySpec(key, aesEncryptionAlgorithm);
		IvParameterSpec ivParameterSpec = new IvParameterSpec(initialVector);
		cipher.init(Cipher.DECRYPT_MODE, secretKeySpecy, ivParameterSpec);
		cipherText = cipher.doFinal(cipherText);
		return cipherText;
	}

	public static byte[] encrypt(byte[] plainText, byte[] key, byte [] initialVector) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException
	{
		Cipher cipher = Cipher.getInstance(cipherTransformation);
		SecretKeySpec secretKeySpec = new SecretKeySpec(key, aesEncryptionAlgorithm);
		IvParameterSpec ivParameterSpec = new IvParameterSpec(initialVector);
		cipher.init(Cipher.ENCRYPT_MODE, secretKeySpec, ivParameterSpec);
		plainText = cipher.doFinal(plainText);
		return plainText;
	}

	private static byte[] getKeyBytes(String key) throws UnsupportedEncodingException{
		byte[] keyBytes= new byte[16];
		byte[] parameterKeyBytes= key.getBytes(characterEncoding);
		System.arraycopy(parameterKeyBytes, 0, keyBytes, 0, Math.min(parameterKeyBytes.length, keyBytes.length));
		return keyBytes;
	}

	/// <summary>
	/// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
	/// </summary>
	/// <param name="plainText">Plain text to encrypt</param>
	/// <param name="key">Secret key</param>
	/// <returns>Base64 encoded string</returns>
	public static String encrypt(String plainText, String key) throws UnsupportedEncodingException, InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException{
		byte[] plainTextbytes = plainText.getBytes(characterEncoding);
		byte[] keyBytes = getKeyBytes(key);
		return Base64.encodeBase64String(encrypt(plainTextbytes,keyBytes, keyBytes));
	}
	
	public static byte[] encrypt(byte[] plaintext, String key) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException, UnsupportedEncodingException {
		byte [] keyBytes = getKeyBytes(key);
		return encrypt(plaintext, keyBytes, keyBytes);
	}

	public static byte [] decrypt(byte[] ciphertext, String key) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException, UnsupportedEncodingException {
		byte [] keybytes = getKeyBytes(key);
		return decrypt(ciphertext, keybytes, keybytes);
	}
	/// <summary>
	/// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
	/// </summary>
	/// <param name="encryptedText">Base64 Encoded String</param>
	/// <param name="key">Secret Key</param>
	/// <returns>Decrypted String</returns>
	public static String decrypt(String encryptedText, String key) throws KeyException, GeneralSecurityException, GeneralSecurityException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException, IOException{
		byte[] cipheredBytes = Base64.decodeBase64(encryptedText);
		byte[] keyBytes = getKeyBytes(key);
		return new String(decrypt(cipheredBytes, keyBytes, keyBytes), characterEncoding);
	}	
}

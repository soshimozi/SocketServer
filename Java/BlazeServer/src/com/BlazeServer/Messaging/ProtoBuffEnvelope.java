package com.BlazeServer.Messaging;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.reflect.Method;
import java.security.GeneralSecurityException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyException;
import java.security.NoSuchAlgorithmException;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import org.bouncycastle.crypto.params.AsymmetricKeyParameter;
import org.bouncycastle.crypto.params.DHPublicKeyParameters;
import org.bouncycastle.crypto.util.PublicKeyFactory;

import com.BlazeServer.Crypto.ServerAuthority;
import com.BlazeServer.Helper.EncryptionHelper;
import com.google.protobuf.Message;

public class ProtoBuffEnvelope extends MessageEnvelope {

	private final MessageRegistry registry;
	private boolean encryptionEnabled = false;
	private ServerAuthority sa = null;
	private AsymmetricKeyParameter remotePublicKey = null;
	
	public ProtoBuffEnvelope(MessageRegistry registry) {
		this.registry = registry;
	}
	
	@Override
	public void Serialize(Object messageObject, OutputStream stream) throws IOException {
		DataOutputStream dos = new DataOutputStream(stream);
		
		Message message = (Message)messageObject;

		if( encryptionEnabled ) {
			
			String privateKey = sa.generateAgreementValue(remotePublicKey).toString(16);
			byte [] buffer = message.toByteArray();
			try {
				
				// read utf8 string
				String encrypted = EncryptionHelper.encrypt(message.getDescriptorForType().getFullName(), privateKey); 
				//dos.writeBytes(encrypted + "\r\n");

				dos.writeUTF(encrypted);
				
				byte [] cipherbuffer = EncryptionHelper.encrypt(buffer, privateKey);
				dos.writeInt(cipherbuffer.length);
				dos.write(cipherbuffer, 0, cipherbuffer.length);
				
			} catch (InvalidKeyException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (NoSuchAlgorithmException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (NoSuchPaddingException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (InvalidAlgorithmParameterException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (IllegalBlockSizeException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (BadPaddingException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			
			
		} else {
		
			dos.writeUTF(message.getDescriptorForType().getFullName());
			
			byte [] buffer = message.toByteArray();
			dos.writeInt(buffer.length);
			dos.write(buffer, 0, buffer.length);
		}

		dos.flush();
	}

	@Override
	public Object Deserialize(InputStream stream) throws IOException {
		
		DataInputStream dis = new DataInputStream(stream);
        String descriptorName = dis.readUTF();

        if( encryptionEnabled ) {
        	
			String privateKey = sa.generateAgreementValue(remotePublicKey).toString(16);
			try {
				descriptorName = EncryptionHelper.decrypt(descriptorName, privateKey);
		        System.out.println(descriptorName);		        

		        byte[] messageBuffer = new byte[dis.readInt()];
		        dis.read(messageBuffer);
				
		        byte[] decryptedBytes = EncryptionHelper.decrypt(messageBuffer, privateKey);
		        
		        Object value = null;
		        try {
					value = objectFromBuffer(descriptorName, decryptedBytes);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
		        
		        return value;
		        
			} catch (KeyException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (InvalidAlgorithmParameterException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (IllegalBlockSizeException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (BadPaddingException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (GeneralSecurityException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
        	
        	return null;
        	
        } else {
	        System.out.println(descriptorName);
	       
	        byte[] messageBuffer = new byte[dis.readInt()];
	        dis.read(messageBuffer);
	        
	        Object value = null;
	        try {
				value = objectFromBuffer(descriptorName, messageBuffer);
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	        
	        return value;
        }
	}
	
	public void enableEncryption(ServerAuthority sa, byte [] publicKey) {
		this.sa = sa;
		
		try {
			remotePublicKey = new DHPublicKeyParameters(
					((DHPublicKeyParameters)PublicKeyFactory.createKey(publicKey)).getY(), 
					sa.getParameters());
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		this.encryptionEnabled = true;
	}
	
	private Object objectFromBuffer(String descriptorName, byte [] message) throws Exception {
		
		Class<?> clazz = registry.getTypeForDescriptor(descriptorName);

		if( clazz != null ) {
			Method parseFromMethod = clazz.getMethod("parseFrom", byte[].class);
			return parseFromMethod.invoke(null, message);
		}
		
		return null;
	}

}

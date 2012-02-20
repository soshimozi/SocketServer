package com.SocketClient.Controller;

import java.io.IOException;

import javax.swing.SwingUtilities;

import com.BlazeServer.ClientEngine;
import com.BlazeServer.Event.ClientConnectedEvent;
import com.BlazeServer.Event.ClientLoginEvent;
import com.BlazeServer.Event.ClientSecureEvent;
import com.BlazeServer.Event.IClientConnectedListener;
import com.BlazeServer.Event.IClientLoginListener;
import com.BlazeServer.Event.IClientSecureListener;
import com.SocketClient.Model.SocketClientModel;
import com.mvc.controller.AbstractController;

public class DefaultController 
		extends AbstractController 
		implements IClientConnectedListener, IClientSecureListener, IClientLoginListener {
	
	public static final String ELEMENT_ADDRESS_PROPERTY = "Address";
	public static final String ELEMENT_PORT_PROPERTY = "Port";
	public static final String ELEMENT_LOGINNAME_PROPERTY = "LoginName";
	public static final String ELEMENT_ENCRYPTIONENABLED_PROPERTY = "EncryptionEnabled";
	public static final String ELEMENT_CONNECTED_PROPERTY = "Connected";
	
	
	private ClientEngine engine = null;
	
    /**
     * Change the address in the model
     * @param newAddress The new address
     */
    public void changeAddress(String newAddress) {
        setModelProperty(ELEMENT_ADDRESS_PROPERTY, newAddress);                                 
    }

    /**
     * Change the port in the model
     * @param newPort The new port
     */
    public void changePort(int newPort) {
    	setModelProperty(ELEMENT_PORT_PROPERTY, newPort);
    }

    /**
     * Change the login name in the model
     * @param newLoginName The new login name
     */
	public void changeLoginName(String newLoginName) {
		setModelProperty(ELEMENT_LOGINNAME_PROPERTY, newLoginName);
	}
    
	/**
	 * Change the encryption enabled in the model
	 * @param newEnable The new enable value
	 */
	public void changeEncryptionEnabled(boolean newEnable) {
		setModelProperty(ELEMENT_ENCRYPTIONENABLED_PROPERTY, newEnable);
	}
	
	public void changeConnected(boolean connected) {
		setModelProperty(ELEMENT_CONNECTED_PROPERTY, connected);
	}
	
	/**
	 * Connect to socket
	 */
    public void buttonConnectClicked() {
    	connectSocket();
    }
    
    private void connectSocket() {
    	
    	boolean connected = (Boolean) getModelProperty(SocketClientModel.class, ELEMENT_CONNECTED_PROPERTY);
    	
    	if( !connected ) {
    		
        	String address = (String)getModelProperty(SocketClientModel.class, ELEMENT_ADDRESS_PROPERTY);
        	int port = (Integer) getModelProperty(SocketClientModel.class, ELEMENT_PORT_PROPERTY);

        	engine = new ClientEngine(address, port);
	    	
			engine.addClientConnectedListener(this);
			engine.addClientSecureListener(this);
			
			Thread engineThread = new Thread(engine);
			engineThread.start();

			changeConnected(true);

    	} else {
    		
    		engine.stop();
    		engine = null;
    		
    		changeConnected(false);
    	}
		
    }

	@Override
	public void clientSecure(ClientSecureEvent event) {
		
		String loginName = (String)getModelProperty(SocketClientModel.class, ELEMENT_LOGINNAME_PROPERTY);
		
		// tell model we are secure
		System.out.println("We are now secure.");
		
		try {
			engine.login(loginName);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}		
	}

	@Override
	public void clientConnected(ClientConnectedEvent event) {
	
		Boolean enableEncryption = (Boolean)getModelProperty(SocketClientModel.class, ELEMENT_ENCRYPTIONENABLED_PROPERTY);
		
		if( enableEncryption )
			// enable encryption
			engine.enableEncryption();
		else {

			String loginName = (String)getModelProperty(SocketClientModel.class, ELEMENT_LOGINNAME_PROPERTY);
			
			// otherwise just login
			try {
				engine.login(loginName);
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}		
			
		}
	}

	@Override
	public void clientLogin(ClientLoginEvent evt) {
		// tell model we are logged in
		System.out.println("We are now logged in.");
		
	}



}

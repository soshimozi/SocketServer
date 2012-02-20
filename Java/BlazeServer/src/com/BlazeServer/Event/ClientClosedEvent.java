package com.BlazeServer.Event;

import java.util.EventObject;

import com.BlazeServer.Network.ClientConnection;

public final class ClientClosedEvent extends EventObject {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private final ClientConnection client;
	
	public ClientClosedEvent(Object source, ClientConnection client) {
		super(source);
		
		// TODO Auto-generated constructor stub
		this.client = client;
	}

	public ClientConnection getClient() {
		return client;
	}

}

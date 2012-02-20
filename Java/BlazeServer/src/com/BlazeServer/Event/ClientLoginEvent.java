package com.BlazeServer.Event;

import java.util.EventObject;

public class ClientLoginEvent extends EventObject {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private final boolean success;
	
	public ClientLoginEvent(Object source, boolean success) {
		super(source);
		
		this.success = success;
	}
	
	public boolean getSuccess() {
		return success;
	}
}

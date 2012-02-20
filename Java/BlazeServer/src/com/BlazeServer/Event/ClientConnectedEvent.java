package com.BlazeServer.Event;

import java.util.EventObject;

public class ClientConnectedEvent extends EventObject {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public ClientConnectedEvent(Object source) {
		super(source);
	}

}

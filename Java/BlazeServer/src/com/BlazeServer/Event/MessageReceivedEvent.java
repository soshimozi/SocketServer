package com.BlazeServer.Event;

import java.util.EventObject;

import com.google.protobuf.Message;

public final class MessageReceivedEvent extends EventObject {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private final Message message;
	
	public MessageReceivedEvent(Object source, Message message) {
		super(source);
		
		// TODO Auto-generated constructor stub
		this.message = message;
	}
	
	public Message getMessage() {
		return message;
	}

}

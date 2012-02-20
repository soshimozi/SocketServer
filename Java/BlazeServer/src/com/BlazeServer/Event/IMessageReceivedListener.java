package com.BlazeServer.Event;

import java.util.EventListener;

public interface IMessageReceivedListener extends EventListener {
	public void messageReceived(MessageReceivedEvent event);
}

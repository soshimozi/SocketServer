package com.BlazeServer.Event;

import java.util.EventListener;

public interface IClientConnectedListener extends EventListener {
	public void clientConnected(ClientConnectedEvent event);

}

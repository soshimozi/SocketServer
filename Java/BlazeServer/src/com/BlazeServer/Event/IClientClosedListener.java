package com.BlazeServer.Event;

import java.util.EventListener;

public interface IClientClosedListener extends EventListener {
	public void clientClosed(ClientClosedEvent event);
}

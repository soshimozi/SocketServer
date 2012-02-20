package com.BlazeServer.Event;

import java.util.EventListener;

public interface IClientSecureListener extends EventListener {
	public void clientSecure(ClientSecureEvent event);
}

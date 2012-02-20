package com.BlazeServer.Event;

import java.util.EventListener;

public interface IClientLoginListener extends EventListener {
	public void clientLogin(ClientLoginEvent evt);
}

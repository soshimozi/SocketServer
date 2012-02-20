package com.BlazeServer.Messaging;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.charset.Charset;

public abstract class MessageEnvelope {
	
	private Charset messageEncoding;
	
	protected MessageEnvelope() {
		messageEncoding = Charset.defaultCharset();
	}
	
	public Charset getMessageEncoding() {
		return messageEncoding;
	}
	
	public void setMessageEncoding(Charset value) {
		messageEncoding = value;
	}
	
	public abstract void Serialize(Object message, OutputStream stream) throws IOException;
	public abstract Object Deserialize(InputStream stream) throws IOException;
}

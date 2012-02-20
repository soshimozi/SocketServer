package com.BlazeServer.Serialization;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStream;

public class OutputStreamWrapper extends OutputStream {

	private final OutputStream wrappedStream;
	private final ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
	
	public OutputStreamWrapper(OutputStream wrapped) {
		this.wrappedStream = wrapped;
	}
	
	@Override
	public void write(int b) throws IOException {
		
		wrappedStream.write(b);
		
	}
	
	@Override
	public void flush() throws IOException {
		
		wrappedStream.flush();
	}
	
	@Override
	public void write(byte[] buffer, int offset, int count) throws IOException {
		wrappedStream.write(buffer, offset, count);
	}
	
	public byte [] getBytes() {
		return outputStream.toByteArray();
	}
	

}

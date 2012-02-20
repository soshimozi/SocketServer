package com.BlazeServer.Serialization;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class InputStreamWrapper extends InputStream {

	private final InputStream wrappedStream;
	private final ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
	
	public InputStreamWrapper(InputStream wrapped) {
		this.wrappedStream = wrapped;
	}
	
	@Override
	public int read() throws IOException {
		return wrappedStream.read();
	}
	
	@Override
	public int read(byte[] buffer, int offset, int count) throws IOException {
		
        byte[] data = new byte[count];
        
        // Read into our temp data structure
        int read = wrappedStream.read(data, 0, count);
        
        // Make sure we write this data into the copy buffer
        outputStream.write(data, 0, read);
        
        // now copy this data into the output buffer
        System.arraycopy(data, 0, buffer, offset, read);
        return read;
        
	}
	
	public byte [] getBytes() {
		return outputStream.toByteArray();
	}
}

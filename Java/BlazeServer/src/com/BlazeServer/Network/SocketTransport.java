package com.BlazeServer.Network;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.net.SocketAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public class SocketTransport implements INetworkTransport {

	private Socket client = null;
	private String address;
	private int port;
	
	public SocketTransport() {
	}
	
	@Override
	public String getAddress() {
		// TODO Auto-generated method stub
		return address;
	}

	@Override
	public void setAddress(String value) {
		address = value;

	}

	@Override
	public int getPort() {
		// TODO Auto-generated method stub
		return port;
	}

	@Override
	public void setPort(int value) {
		// TODO Auto-generated method stub
		port = value;

	}

	@Override
	public SocketAddress getSocketAddress() {
		// TODO Auto-generated method stub
		return client.getRemoteSocketAddress();
	}


	@Override
	public void Connect() throws UnknownHostException, IOException {
		// TODO Auto-generated method stub
		if(client != null) return;
		
		client = new Socket(address, port);
	}

	@Override
	public void Disconnect(boolean force) throws IOException {
		// TODO Auto-generated method stub
		client.close();
	}

	@Override
	public boolean getIsConnected() {
		// TODO Auto-generated method stub
		return client != null && client.isConnected();
	}

	@Override
	public boolean getHasConnection() {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public InputStream getInStream() throws IOException {
		// TODO Auto-generated method stub
		return client.getInputStream();
	}

	@Override
	public OutputStream getOutStream() throws IOException {
		// TODO Auto-generated method stub
		return client.getOutputStream();
	}

	@Override
	public int getReceiveTimeout() throws SocketException {
		// TODO Auto-generated method stub
		return client.getSoTimeout();
	}

	@Override
	public void setReceiveTimeout(int value) throws SocketException {
		// TODO Auto-generated method stub
		client.setSoTimeout(value);

	}

}

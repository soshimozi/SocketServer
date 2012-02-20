package com.BlazeServer.Network;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.SocketAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public interface INetworkTransport {

	public String getAddress(); 
    public void setAddress(String value);

    public int getPort();
    public void setPort(int value);
    	
    public SocketAddress getSocketAddress();

    public void Connect() throws UnknownHostException, IOException;
    
    public void Disconnect(boolean force) throws IOException;

    public  boolean getIsConnected();
    
    public boolean getHasConnection();
    
    public InputStream getInStream() throws IOException;
    public OutputStream getOutStream() throws IOException;
    
    public int getReceiveTimeout() throws SocketException;
    public void setReceiveTimeout(int value) throws SocketException;
}

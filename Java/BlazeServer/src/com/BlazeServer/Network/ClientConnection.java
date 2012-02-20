package com.BlazeServer.Network;

import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.net.UnknownHostException;

import javax.swing.event.EventListenerList;

import com.BlazeServer.Event.ClientClosedEvent;
import com.BlazeServer.Event.IClientClosedListener;
import com.BlazeServer.Event.IMessageReceivedListener;
import com.BlazeServer.Event.MessageReceivedEvent;
import com.BlazeServer.Messaging.MessageDispatcher;
import com.BlazeServer.Messaging.MessageEnvelope;
import com.BlazeServer.Serialization.InputStreamWrapper;
import com.BlazeServer.Serialization.OutputStreamWrapper;
import com.google.protobuf.Message;

public final class ClientConnection implements Runnable {
	
	private final INetworkTransport client;
	private MessageEnvelope envelope = null;
	private Thread responderThread;

	private volatile boolean running = false;
	private final EventListenerList eventListeners = new EventListenerList();
	private final MessageDispatcher dispatcher;
	
	public ClientConnection(MessageEnvelope envelope, INetworkTransport client, MessageDispatcher dispatcher) {
		this.client = client;
		this.envelope = envelope;
		this.dispatcher = dispatcher;
	}
	
	public void Connect(String serverAddress, int port) throws UnknownHostException, IOException {
		client.setAddress(serverAddress);
		client.setPort(port);
		client.Connect();
		
		startReceiveThread();
	}

	public void Connect() throws UnknownHostException, IOException {
		client.Connect();
		
		startReceiveThread();
	}
	
	public synchronized INetworkTransport getTransport() {
		return client;
	}
	
	public synchronized MessageEnvelope getEnvelope() {
		return envelope;
	}

	public synchronized void addMessageReceivedListener(IMessageReceivedListener listener) {
		eventListeners.add(IMessageReceivedListener.class, listener);
	}
	
	public synchronized void removeMessageRecievedListener(IMessageReceivedListener listener) {
		eventListeners.remove(IMessageReceivedListener.class, listener);
	}
	
	public synchronized void addClientClosedListener(IClientClosedListener listener) {
		eventListeners.add(IClientClosedListener.class, listener);
	}
	
	public synchronized void removeClientClosedListener(IClientClosedListener listener) {
		eventListeners.remove(IClientClosedListener.class, listener);
	}

	protected void fireClientClosed() {
		ClientClosedEvent event = new ClientClosedEvent(this, this);

		IClientClosedListener[] listeners = eventListeners.getListeners(IClientClosedListener.class);
		for (int i = 0; i < listeners.length; i++) {
			listeners[i].clientClosed(event);
	    }		
	}
	
	protected void fireMessageRecieved(Message message) {
		MessageReceivedEvent event = new MessageReceivedEvent(this, message);
		
		IMessageReceivedListener[] listeners = eventListeners.getListeners(IMessageReceivedListener.class);
		for (int i = 0; i < listeners.length; i++) {
			listeners[i].messageReceived(event);
	    }		
	}
	
	private void startReceiveThread() {
		if( running ) return;
		responderThread = new Thread(this);
		responderThread.start();
	}
	

	@Override
	public void run() {
		running = true;
		
		while(running) {
			
			InputStreamWrapper stream = null;
			try {
				stream = new InputStreamWrapper(client.getInStream());
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			
			if( null != stream ) {
				
				Message message = null;
				
				try {
					
					message = (Message) envelope.Deserialize(stream);
					
					if( null != message ) {
						dispatcher.invokeHandlerForMessage(this, message);
						//fireMessageRecieved(message);
					}
					
					// dispatch message
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
					
					// IOException is from socket
					fireClientClosed();
					running = false;
					
				} catch (IllegalArgumentException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (IllegalAccessException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (InvocationTargetException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				
				//if( message != null ) {
				//}
			}
		}
	
	}
	
	public synchronized void Disconnect() throws InterruptedException, IOException {
		client.Disconnect(true);
		//running = false;
		//responderThread.join();
	}
	
    public static ClientConnection CreateClientConnection(MessageEnvelope envelope, INetworkTransport client, MessageDispatcher dispatcher) {
        ClientConnection connection = new ClientConnection(envelope, client, dispatcher);
        connection.startReceiveThread();

        return connection;
    }	
    
    public void Send(Message message) throws IOException {
    	
    	if( message == null )
    		throw new NullPointerException("Attempt to send a null message");
    	
    	OutputStreamWrapper stream = new OutputStreamWrapper(client.getOutStream());
    	envelope.Serialize(message, stream);
    }

	public boolean isConnected() {
		// TODO Auto-generated method stub
		return client.getIsConnected();
	}

}

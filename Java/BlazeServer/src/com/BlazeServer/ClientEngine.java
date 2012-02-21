package com.BlazeServer;

import java.io.IOException;
import java.math.BigInteger;
import java.net.UnknownHostException;

import javax.swing.event.EventListenerList;

import com.BlazeServer.Attributes.MessageHandler;
import com.BlazeServer.Crypto.ServerAuthority;
import com.BlazeServer.Event.*;
import com.BlazeServer.Messages.MessageProtos;
import com.BlazeServer.Messages.MessageProtos.CreateRoomVariableRequest;
import com.BlazeServer.Messages.MessageProtos.EnableEncryptionRequest;
import com.BlazeServer.Messages.MessageProtos.EnableEncryptionResponse;
import com.BlazeServer.Messages.MessageProtos.JoinRoomEvent;
import com.BlazeServer.Messages.MessageProtos.LoginRequest;
import com.BlazeServer.Messages.MessageProtos.LoginResponse;
import com.BlazeServer.Messages.MessageProtos.RoomVariable;
import com.BlazeServer.Messages.MessageProtos.ServerConnectionResponse;
import com.BlazeServer.Messaging.MessageDispatcher;
import com.BlazeServer.Messaging.MessageRegistry;
import com.BlazeServer.Messaging.ProtoBuffEnvelope;
import com.BlazeServer.Network.ClientConnection;
import com.BlazeServer.Network.SocketTransport;
import com.google.protobuf.ByteString;
import java.util.logging.Level;
import java.util.logging.Logger;

public class ClientEngine implements 
	IClientClosedListener, 
	Runnable
{
    private final String address;
    private final int port;

    private final Object serverEvent = new Object(); 
    private final EventListenerList eventListeners = new EventListenerList();

    private ClientConnection connection;
    private MessageDispatcher dispatcher;
    private ServerAuthority sa;

    public ClientEngine(String address, int port) {
        this.address = address;
        this.port = port;
    }

    private volatile boolean running = false;
    private volatile boolean connected = false;

    public boolean getRunning() {
        return running;
    }

    /**
        * @return the connected
        */
    public boolean isConnected() {
            return connected;
    }

    @Override
    public void run() {

            // create the dispatcher and tell it to scan this class
            dispatcher = new MessageDispatcher(this);

            connection = new ClientConnection(
                            new ProtoBuffEnvelope(new MessageRegistry("/messages.desc")), 
                            new SocketTransport(),
                            dispatcher);

            connection.addClientClosedListener(this);

            try {
                connection.Connect(address, port);
            } catch (UnknownHostException e) {
                    e.printStackTrace();
            } catch (IOException e) {
                    e.printStackTrace();
            }

            if( connection.isConnected() ) {
                running = true;
                while(running) {
                    synchronized(serverEvent) {
                        try {
                            serverEvent.wait();
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                }

                System.out.println("ClientEngine stopping");
                if( connection.isConnected() ) {
                    try {
                        connection.Disconnect();
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
            }
            
            fireClientStopped();
    }

    public synchronized void login(String userName) throws IOException {
        LoginRequest.Builder newRequest = LoginRequest.newBuilder();
        newRequest.setUserName(userName);

        connection.Send(newRequest.build());
    }

    public synchronized void enableEncryption() {

        EnableEncryptionRequest.Builder newRequest = EnableEncryptionRequest.newBuilder();
        newRequest.setMessageId(234);
        newRequest.setEnable(true).setPublickey(ByteString.copyFrom(sa.generateEncodedPublicKeyInfo()));

        try {
                connection.Send(newRequest.build());
        } catch (IOException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
        }

    }

    public synchronized void stop() {

        running = false;

        synchronized(serverEvent) {
                serverEvent.notify();
        }

        // join thread?
    }
    
    public synchronized void createRoomVariable(String name, String value) {
        String roomName = ""; // TODO: get room name from helper
        
        RoomVariable.Builder newVariable = RoomVariable.newBuilder();
        newVariable.setName(name).setValue(value);
        
        CreateRoomVariableRequest.Builder newRequest = CreateRoomVariableRequest.newBuilder();
        newRequest.setVariable(newVariable).setRoomName(roomName);
        try {
            connection.Send(newRequest.build());
        } catch (IOException ex) {
            Logger.getLogger(ClientEngine.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    @Override
    public void clientClosed(ClientClosedEvent event) {

        running = false;

        synchronized(serverEvent) {
            serverEvent.notify();
        }
    }

    @MessageHandler("message.JoinRoomEvent")
    public void handleLoginResponse(ClientConnection connection, JoinRoomEvent event) {
        // server notified us that we joined a room
    }

        
    @MessageHandler("message.LoginResponse")
    public void handleLoginResponse(ClientConnection connection, LoginResponse response) {
        
        // server notified us with a login response
        if( !response.getSuccess() ) {
            try {
                connection.Disconnect();
            } catch (InterruptedException ex) {
                Logger.getLogger(ClientEngine.class.getName()).log(Level.SEVERE, null, ex);
            } catch (IOException ex) {
                Logger.getLogger(ClientEngine.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }
	
	
    @MessageHandler("message.EnableEncryptionResponse")
    public void handleEnableEncryptionResponse(ClientConnection connection, EnableEncryptionResponse response) {

        if( response.getSuccess() ) {
            ((ProtoBuffEnvelope)connection.getEnvelope()).enableEncryption(sa, response.getPublickey().toByteArray());
        }

        fireClientSecure();
    }

    @MessageHandler("message.ServerConnectionResponse")
    public void handleServerConnectionResponse(ClientConnection connection, ServerConnectionResponse response) {
        // we have the parameter values in the response
        sa = new ServerAuthority(
                        new BigInteger(response.getParameters().getP(), 16),
                        new BigInteger(response.getParameters().getG(), 16));

        connected = true;
        fireClientConnected();
    }

    public synchronized void addClientConnectedListener(IClientConnectedListener listener) {
        eventListeners.add(IClientConnectedListener.class, listener);
    }

    public synchronized void removeClientClosedListener(IClientConnectedListener listener) {
        eventListeners.remove(IClientConnectedListener.class, listener);
    }

    public synchronized void addClientSecureListener(IClientSecureListener listener) {
        eventListeners.add(IClientSecureListener.class, listener);
    }

    public synchronized void removeClientSecureListener(IClientSecureListener listener) {
        eventListeners.remove(IClientSecureListener.class, listener);
    }

    public synchronized void addClientLoginListener(IClientLoginListener listener) {
        eventListeners.add(IClientLoginListener.class, listener);
    }

    public synchronized void removeClientLoginListener(IClientLoginListener listener) {
        eventListeners.remove(IClientLoginListener.class, listener);
    }
	
    public synchronized void addClientStoppedListener(IClientStoppedListener listener) {
        eventListeners.add(IClientStoppedListener.class, listener);
    }

    public synchronized void removeClientStoppedListener(IClientStoppedListener listener) {
        eventListeners.remove(IClientStoppedListener.class, listener);
    }
	
    protected void fireClientStopped() {
        ClientStoppedEvent event = new ClientStoppedEvent(this);

        IClientStoppedListener[] listeners = eventListeners.getListeners(IClientStoppedListener.class);
        for (int i = 0; i < listeners.length; i++) {
            listeners[i].engineStopped(event);
        }		
    }

    protected void fireClientLogin(boolean success) {
        ClientLoginEvent event = new ClientLoginEvent(this, success);

        IClientLoginListener[] listeners = eventListeners.getListeners(IClientLoginListener.class);
        for (int i = 0; i < listeners.length; i++) {
            listeners[i].clientLogin(event);
        }		
    }

    protected void fireClientConnected() {
        ClientConnectedEvent event = new ClientConnectedEvent(this);

        IClientConnectedListener[] listeners = eventListeners.getListeners(IClientConnectedListener.class);
        for (int i = 0; i < listeners.length; i++) {
            listeners[i].clientConnected(event);
        }		
    }
	
    protected void fireClientSecure() {
        ClientSecureEvent event = new ClientSecureEvent(this);

        IClientSecureListener[] listeners = eventListeners.getListeners(IClientSecureListener.class);
        for (int i = 0; i < listeners.length; i++) {
            listeners[i].clientSecure(event);
        }		
    }
}

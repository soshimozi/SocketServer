import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.math.BigInteger;
import java.net.UnknownHostException;

import com.BlazeServer.ClientEngine;
import com.BlazeServer.Attributes.MessageHandler;
import com.BlazeServer.Crypto.ServerAuthority;
import com.BlazeServer.Event.ClientClosedEvent;
import com.BlazeServer.Event.ClientConnectedEvent;
import com.BlazeServer.Event.ClientSecureEvent;
import com.BlazeServer.Event.IClientClosedListener;
import com.BlazeServer.Event.IClientConnectedListener;
import com.BlazeServer.Event.IClientSecureListener;
import com.BlazeServer.Event.IMessageReceivedListener;
import com.BlazeServer.Event.MessageReceivedEvent;
import com.BlazeServer.Messages.MessageProtos.EnableEncryptionRequest;
import com.BlazeServer.Messages.MessageProtos.EnableEncryptionResponse;
import com.BlazeServer.Messages.MessageProtos.LoginRequest;
import com.BlazeServer.Messages.MessageProtos.ServerConnectionResponse;
import com.BlazeServer.Messages.MessageProtos.TestMessage;
import com.BlazeServer.Messaging.MessageDispatcher;
import com.BlazeServer.Messaging.MessageRegistry;
import com.BlazeServer.Messaging.ProtoBuffEnvelope;
import com.BlazeServer.Network.ClientConnection;
import com.BlazeServer.Network.SocketTransport;
import com.google.protobuf.ByteString;

public class myClient 
	implements  IClientConnectedListener, IClientSecureListener {
	
	/**
	 * @param args
	 */
	public static void main(String[] args) {
		
		myClient program = new myClient();
		program.run(args);
		
	}

	ClientEngine engine = null;
	public void run(String[] args) {
		
		engine = new ClientEngine("localhost", 4000);
		engine.addClientConnectedListener(this);
		engine.addClientSecureListener(this);
		
		//Thread engineThread = new Thread(engine);
		//engineThread.start();
	}


	@Override
	public void clientConnected(ClientConnectedEvent event) {
		// TODO Auto-generated method stub
		System.out.println("Here is were we would use the UI to ask for a user name");
		
	}


	@Override
	public void clientSecure(ClientSecureEvent event) {
		// TODO Auto-generated method stub
		
		// here is were we tell the world that we can now send our data
		// until we get here we are still handshaking
		System.out.println("We are now secure.");
		
		try {
			engine.login("Soshimo");
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
}

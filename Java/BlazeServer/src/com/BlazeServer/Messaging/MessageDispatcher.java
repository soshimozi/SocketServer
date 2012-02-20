package com.BlazeServer.Messaging;

import java.lang.annotation.Annotation;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;

import com.BlazeServer.Attributes.MessageHandler;
import com.BlazeServer.Network.ClientConnection;
import com.google.protobuf.Message;


public class MessageDispatcher {

	private final Map<String, Method> handlerMap = new HashMap<String, Method>();
	private final Object listener;
	
	public MessageDispatcher(Object listener) throws NullPointerException {
		
		if( listener == null ) throw new NullPointerException("listener");
		
		this.listener = listener;
		
		Class<? extends Object> clz = listener.getClass();
		for( Method method : clz.getDeclaredMethods()) {
			
			Annotation annotation = method.getAnnotation(MessageHandler.class);
			if( annotation != null && annotation instanceof MessageHandler ) {
				handlerMap.put(((MessageHandler) annotation).value(), method);
			}
		}
	}
	
	public void invokeHandlerForMessage(ClientConnection connection, Message message) throws IllegalArgumentException, IllegalAccessException, InvocationTargetException {
		
		String descriptorName = message.getDescriptorForType().getFullName();
		if(handlerMap.containsKey(descriptorName)) {
			handlerMap.get(descriptorName).invoke(listener, connection, message);
		}
	}
}

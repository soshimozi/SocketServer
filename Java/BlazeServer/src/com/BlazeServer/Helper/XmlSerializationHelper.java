package com.BlazeServer.Helper;

import java.io.StringWriter;

import org.simpleframework.xml.Serializer;
import org.simpleframework.xml.core.Persister;

public class XmlSerializationHelper {
	public static <T> T readObject(Class <T> c, String str) throws Exception {
	  	  Serializer serializer = new Persister();
	  	  return serializer.read(c, str);
		}

	public static <T> String serializeObject(Class<T> c,
			T object) throws Exception {
		
		// TODO Auto-generated method stub
		Serializer serializer = new Persister();
		StringWriter writer = new StringWriter();
  	  	
  	  	serializer.write(object, writer);		
		
		return writer.toString();
		
		
	}

}

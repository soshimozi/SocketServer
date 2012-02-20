package com.BlazeServer.Messaging;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;

import com.google.protobuf.DescriptorProtos.FileDescriptorProto;
import com.google.protobuf.DescriptorProtos.FileDescriptorSet;
import com.google.protobuf.Descriptors.Descriptor;
import com.google.protobuf.Descriptors.DescriptorValidationException;
import com.google.protobuf.Descriptors.FileDescriptor;

public class MessageRegistry {
	private final Map<String, String> mapping = new HashMap<String, String>();

	
	public MessageRegistry(String descriptorPath) {
		
		mapDescriptors(descriptorPath);

	}


	
	private void mapDescriptors(String descriptorPath) {
		FileDescriptorSet descriptorSet = null;
		try {
			
			InputStream stream = this.getClass().getResourceAsStream(descriptorPath);
			
			if( stream != null ) {
				descriptorSet = FileDescriptorSet.parseFrom(stream);
			}
			
		} catch (FileNotFoundException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		} catch (IOException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
		
		if( descriptorSet != null ) {
			for (FileDescriptorProto fdp: descriptorSet.getFileList()) {
				
				FileDescriptor fd = null;
				try {
					fd = FileDescriptor.buildFrom(fdp, new FileDescriptor[]{});
				} catch (DescriptorValidationException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				
				for (Descriptor descriptor : fd.getMessageTypes()) {
					String className = fdp.getOptions().getJavaPackage() + "."
						+ fdp.getOptions().getJavaOuterClassname() + "$"
						+ descriptor.getName();
					
					mapping.put(descriptor.getFullName(), className);
				}
			}
		}
	}
	
	public Class<?> getTypeForDescriptor(String descriptorName) throws ClassNotFoundException {
		
		if( mapping.containsKey(descriptorName)) {
			return Thread.currentThread().getContextClassLoader()
					.loadClass(mapping.get(descriptorName));		
		}
		
		return null;
	}
	
}

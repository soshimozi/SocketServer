package com.SocketClient.Model;

import com.SocketClient.Controller.DefaultController;
import com.mvc.model.AbstractModel;

public class SocketClientModel extends AbstractModel {

	private String address;
	private int port;
	private String loginName;
	private boolean encryptionEnabled;
	private boolean connected;
	
	public void initDefault() {
		setAddress("");
		setPort(0);
		setLoginName("");
		setEncryptionEnabled(false);
		setConnected(false);
	}

	public String getLoginName() {
		return loginName;
	}
	
	public void setLoginName(String loginName) {
		String oldLoginName = this.loginName;
		this.loginName = loginName;
		
		firePropertyChange(DefaultController.ELEMENT_LOGINNAME_PROPERTY, oldLoginName, loginName);
	}
	
	/**
	 * @return the address
	 */
	public String getAddress() {
		return address;
	}

	/**
	 * @param address the address to set
	 */
	public void setAddress(String address) {
		
		String oldAddress = this.address;
		this.address = address;
		
		firePropertyChange(DefaultController.ELEMENT_ADDRESS_PROPERTY, oldAddress, address);
	}

	/**
	 * @return the port
	 */
	public Integer getPort() {
		return port;
	}

	/**
	 * @param port the port to set
	 */
	public void setPort(Integer port) {
		int oldPort = this.port;
		this.port = port;
		
		firePropertyChange(DefaultController.ELEMENT_PORT_PROPERTY, oldPort, port);
	}
	
	public Boolean getEncryptionEnabled() {
		return encryptionEnabled;
	}
	
	public void setEncryptionEnabled(Boolean enabled) {
		boolean oldEnabled = this.encryptionEnabled;
		this.encryptionEnabled = enabled;
		
		firePropertyChange(DefaultController.ELEMENT_ENCRYPTIONENABLED_PROPERTY, oldEnabled, enabled);
	}
	
	public Boolean getConnected() {
		return connected;
	}
	
	public void setConnected(Boolean connected) {
		boolean oldValue = this.connected;
		this.connected = connected;
		
		firePropertyChange(DefaultController.ELEMENT_CONNECTED_PROPERTY, oldValue, connected);
	}
	
	
}

package com.SocketClient.View;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.beans.PropertyChangeEvent;

import com.SocketClient.Controller.DefaultController;
import com.mvc.view.AbstractViewPanel;


public class TrafficSignalView extends AbstractViewPanel {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	Color on;
	int radius = 8;
	int border = 1;
	boolean active = false;
	
	public TrafficSignalView() {
		on = Color.red;
		active = true;
	}
	
	public Dimension getPreferredSize() {
		int size = (radius+border)*2;
		return new Dimension(size, size);
	}
	
	public void paintComponent(Graphics g) {
		g.setColor(Color.black);
		g.fillRect(0, 0, getWidth(), getHeight());
		
		if( active ) {
			g.setColor(on);
		} else {
			g.setColor(on.darker().darker().darker());
		}
		
		g.fillOval(border,  border, 2*radius, 2*radius);
	}

	@Override
	public void modelPropertyChange(PropertyChangeEvent evt) {
		// TODO Auto-generated method stub
		if (evt.getPropertyName().equals(DefaultController.ELEMENT_CONNECTED_PROPERTY)) {
			
			// change active based on connected property
			Boolean connectedValue = (Boolean)evt.getNewValue();
			if( connectedValue ) {
				on = Color.green;
				repaint();
			} else {
				on = Color.red;
				repaint();
			}
            
        }
		
	}
}

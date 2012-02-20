package com.SocketClient.View;

import java.awt.Color;
import java.awt.Graphics;
import java.beans.PropertyChangeEvent;

import com.mvc.view.AbstractViewPanel;

public class Marker extends AbstractViewPanel {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private int x=0;
    private int y=0;
    
    private Color col;
    private boolean dispose=false;
    
    public Marker() {
        super();
    }
    
    public Marker(Color c) {
    	super();
    	this.col = c;
    }
    
    public void paintComponent(Graphics g) {
    	g.setColor(col);
    	g.fillOval(x, y, 9, 9);
    	g.setColor(Color.BLACK);
    	g.drawOval(x, y, 8, 8);
    }
    
    public void setColor(Color c) {
    	this.col = c;
    	repaint();
    }
    
    public void setCoord(int x, int y) {
    	this.x = x;
    	this.y = y;
    	repaint();
    }
    
    public void dispose() {
    	dispose = true;
    }
    
    
	    
	@Override
	public void modelPropertyChange(PropertyChangeEvent evt) {
		// TODO Auto-generated method stub

	}

}

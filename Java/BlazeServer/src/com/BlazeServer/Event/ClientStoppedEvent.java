/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.BlazeServer.Event;

import java.util.EventObject;

/**
 *
 * @author MonkeyBreath
 */
public class ClientStoppedEvent extends EventObject {
    
    public ClientStoppedEvent(Object source) {
        super(source);
    }
}

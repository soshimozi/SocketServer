/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.BlazeServer.Event;

import java.util.EventListener;

/**
 *
 * @author MonkeyBreath
 */
public interface IClientStoppedListener extends EventListener {
    public void engineStopped(ClientStoppedEvent event);
}

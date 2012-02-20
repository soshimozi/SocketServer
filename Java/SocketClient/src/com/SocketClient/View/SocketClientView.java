package com.SocketClient.View;

import java.awt.Color;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.beans.PropertyChangeEvent;

import javax.swing.AbstractButton;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTabbedPane;
import javax.swing.JTextField;
import javax.swing.JTextPane;

import com.SocketClient.Controller.DefaultController;
import com.jgoodies.forms.factories.FormFactory;
import com.jgoodies.forms.layout.ColumnSpec;
import com.jgoodies.forms.layout.FormLayout;
import com.jgoodies.forms.layout.RowSpec;
import com.mvc.view.AbstractViewPanel;

public class SocketClientView extends AbstractViewPanel {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private JTextField textAddress;
	private JTextField textPort;
	private JTextField textLoginName;
	private JCheckBox chkEnableEncryption;
	private JButton btnConnect;
	
	private DefaultController controller;

	/**
	 * Create the frame.
	 */
	public SocketClientView(DefaultController controller) {
		
		this.controller = controller;
		
		initComponents();
	    localInitialization();
	}

	@Override
	public void modelPropertyChange(PropertyChangeEvent evt) {

		if (evt.getPropertyName().equals(DefaultController.ELEMENT_ADDRESS_PROPERTY)) {
            
            String newStringValue = evt.getNewValue().toString();
            
            if (!textAddress.getText().equals(newStringValue))
                textAddress.setText(newStringValue);
            
        }

        else if (evt.getPropertyName().equals(DefaultController.ELEMENT_PORT_PROPERTY)) {

            String newStringValue = evt.getNewValue().toString();
            
            if(!textPort.getText().equals(newStringValue))
            	textPort.setText(newStringValue);
        }
		
        else if (evt.getPropertyName().equals(DefaultController.ELEMENT_LOGINNAME_PROPERTY)) {

            String newStringValue = evt.getNewValue().toString();
            
            if(!textLoginName.getText().equals(newStringValue))
            	textLoginName.setText(newStringValue);
        }

        else if (evt.getPropertyName().equals(DefaultController.ELEMENT_ENCRYPTIONENABLED_PROPERTY)) {

            boolean newSelected = (Boolean)evt.getNewValue();
            boolean selected = chkEnableEncryption.getModel().isSelected();
            
            if( newSelected != selected )
            	chkEnableEncryption.setSelected(newSelected);
        }

        else if (evt.getPropertyName().equals(DefaultController.ELEMENT_CONNECTED_PROPERTY)) {

        	boolean connected = (Boolean)evt.getNewValue();
        	
        	if( connected )
        		btnConnect.setText("Disconnect");
        	else
        		btnConnect.setText("Connect");

        	//chkEnableEncryption.setEnabled(!connected);
        }
	}
	
    
	/**
	 * Used to provide local initialization of Swing components outside of the
	 * NetBeans auto code generator.
	 */
	public void localInitialization() {
	    
	}	
	
	public TrafficSignalView trafficSignal;
	
	private void initComponents() {
		
		setLayout(new FormLayout(new ColumnSpec[] {
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("502px"),},
			new RowSpec[] {
				FormFactory.RELATED_GAP_ROWSPEC,
				RowSpec.decode("406px"),
				FormFactory.RELATED_GAP_ROWSPEC,
				FormFactory.DEFAULT_ROWSPEC,}));
		
		JTabbedPane tabbedPane = new JTabbedPane(JTabbedPane.TOP);
		add(tabbedPane, "2, 2, fill, fill");
		
		JPanel panelConnection = new JPanel();
		tabbedPane.addTab("Connection", null, panelConnection, null);
		panelConnection.setLayout(new FormLayout(new ColumnSpec[] {
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("max(57dlu;default)"),
				ColumnSpec.decode("left:224dlu:grow"),
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("max(12dlu;default):grow"),
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("max(16dlu;default)"),
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,},
			new RowSpec[] {
				FormFactory.RELATED_GAP_ROWSPEC,
				FormFactory.DEFAULT_ROWSPEC,
				FormFactory.RELATED_GAP_ROWSPEC,
				FormFactory.DEFAULT_ROWSPEC,
				FormFactory.RELATED_GAP_ROWSPEC,
				FormFactory.DEFAULT_ROWSPEC,
				FormFactory.RELATED_GAP_ROWSPEC,
				RowSpec.decode("max(33dlu;default):grow"),
				FormFactory.RELATED_GAP_ROWSPEC,
				RowSpec.decode("max(156dlu;default):grow"),}));
		
		JLabel lblAddress = new JLabel("Address:");
		panelConnection.add(lblAddress, "2, 2, right, default");
		
		textAddress = new JTextField();
		panelConnection.add(textAddress, "3, 2, fill, default");
		textAddress.setColumns(10);
		
		JLabel lblPort = new JLabel("Port:");
		panelConnection.add(lblPort, "2, 4, right, default");
		
		textPort = new JTextField();
		panelConnection.add(textPort, "3, 4, left, default");
		textPort.setColumns(10);
		
		JLabel lblLoginName = new JLabel("Login Name:");
		panelConnection.add(lblLoginName, "2, 6, right, default");
		
		textLoginName = new JTextField();
		panelConnection.add(textLoginName, "3, 6, fill, default");
		textLoginName.setColumns(10);
		
		trafficSignal = new TrafficSignalView();
		panelConnection.add(trafficSignal, "2, 8, right, top");
		
		JPanel panel = new JPanel();
		panelConnection.add(panel, "3, 8, left, top");
		panel.setLayout(new FormLayout(new ColumnSpec[] {
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("default:grow"),
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,
				FormFactory.RELATED_GAP_COLSPEC,
				FormFactory.DEFAULT_COLSPEC,},
			new RowSpec[] {
				FormFactory.RELATED_GAP_ROWSPEC,
				FormFactory.DEFAULT_ROWSPEC,
				FormFactory.RELATED_GAP_ROWSPEC,
				RowSpec.decode("default:grow"),}));
		
		chkEnableEncryption = new JCheckBox("Secure Connection");
		panel.add(chkEnableEncryption, "2, 2");
		
		chkEnableEncryption.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				
				 AbstractButton abstractButton = (AbstractButton)e.getSource();
			     boolean selected = abstractButton.getModel().isSelected();
			     controller.changeEncryptionEnabled(selected);
			}
		});
		
		btnConnect = new JButton("Connect");
		btnConnect.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent evt) {
				controller.buttonConnectClicked();
			}
		});
		
		panel.add(btnConnect, "14, 2");
		
		JPanel panel_1 = new JPanel();
		panelConnection.add(panel_1, "3, 10, fill, fill");
		
		Marker redMarker = new Marker(Color.RED);
		panel_1.add(redMarker);
		
		Marker blueMarker = new Marker(Color.BLUE);
		panel_1.add(blueMarker);
		
		JPanel panelOutput = new JPanel();
		tabbedPane.addTab("Output", null, panelOutput, null);
		panelOutput.setLayout(new FormLayout(new ColumnSpec[] {
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("default:grow"),},
			new RowSpec[] {
				FormFactory.RELATED_GAP_ROWSPEC,
				RowSpec.decode("default:grow"),}));
		
		JTextPane textPane = new JTextPane();
		panelOutput.add(textPane, "2, 2, fill, fill");
		
		textAddress.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
            	controller.changeAddress(textAddress.getText());
            }
        });
        
		textAddress.addFocusListener(new java.awt.event.FocusAdapter() {
            public void focusLost(java.awt.event.FocusEvent evt) {
                controller.changeAddress(textAddress.getText());
            }
        });
		
		textPort.addActionListener( new java.awt.event.ActionListener()  {
			public void actionPerformed(java.awt.event.ActionEvent evt) {
				try {
				controller.changePort(
						Integer.parseInt(textPort.getText())
						);
				} catch (Exception e) {
	            //  Handle exception
				}				
			}
			
		});
		
		textPort.addFocusListener(new java.awt.event.FocusAdapter() {
			public void focusLost(java.awt.event.FocusEvent evt) {
				try {
				controller.changePort(
						Integer.parseInt(textPort.getText())
						);
				} catch (Exception e) {
	            //  Handle exception
				}				
			}
		});
		
		textLoginName.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
            	controller.changeLoginName(textLoginName.getText());
            }
        });
        
		textLoginName.addFocusListener(new java.awt.event.FocusAdapter() {
            public void focusLost(java.awt.event.FocusEvent evt) {
                controller.changeLoginName(textLoginName.getText());
            }
        });		
		
	}

}

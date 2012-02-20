import java.awt.BorderLayout;

import javax.swing.JFrame;
import javax.swing.WindowConstants;

import com.SocketClient.Controller.DefaultController;
import com.SocketClient.Model.SocketClientModel;
import com.SocketClient.View.SocketClientView;



public class Main {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub

		SocketClientModel socketClientModel = new SocketClientModel();
		DefaultController controller = new DefaultController();
		SocketClientView socketClientView = new SocketClientView(controller);
		
		controller.addView(socketClientView);
		controller.addView(socketClientView.trafficSignal);
		controller.addModel(socketClientModel);

		socketClientModel.initDefault();
        
        JFrame displayFrame = new JFrame("Socket Client");
        displayFrame.getContentPane().add(socketClientView, BorderLayout.CENTER);
        displayFrame.setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
        displayFrame.pack();
        
        displayFrame.setVisible(true);
	}

}

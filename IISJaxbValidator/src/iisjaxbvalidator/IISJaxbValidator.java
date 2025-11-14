package iisjaxbvalidator;

import java.io.File;
import javax.swing.JFrame;
import javax.swing.SwingUtilities;

/**
 *
 * @author perop
 */
public class IISJaxbValidator {
    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            JFrame frame = new JFrame("JAXB XML Validator");
            frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.setContentPane(new ValidationForm());
            frame.setSize(600, 400);
            frame.setLocationRelativeTo(null);
            frame.setVisible(true);
        });
    }
}

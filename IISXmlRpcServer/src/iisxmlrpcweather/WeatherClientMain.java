/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package iisxmlrpcweather;

import javax.swing.JFrame;
import javax.swing.SwingUtilities;

/**
 *
 * @author perop
 */
public class WeatherClientMain {
    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            JFrame frame = new JFrame("Croatia Weather Lookup");
            frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.setContentPane(new WeatherClientForm());
            frame.setSize(700, 500);
            frame.setLocationRelativeTo(null); 
            frame.setVisible(true);
        });
    }
}

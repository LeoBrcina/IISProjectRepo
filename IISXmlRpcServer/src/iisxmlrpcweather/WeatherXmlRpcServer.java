package iisxmlrpcweather;

import org.apache.xmlrpc.webserver.WebServer;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;

public class WeatherXmlRpcServer {

    public static void main(String[] args) {
        try {
            int port = 80;

            WebServer webServer = new WebServer(port);
            XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

            PropertyHandlerMapping phm = new PropertyHandlerMapping();
            phm.addHandler("WeatherService", WeatherServiceHandler.class);

            xmlRpcServer.setHandlerMapping(phm);
            XmlRpcServerConfigImpl config = (XmlRpcServerConfigImpl) xmlRpcServer.getConfig();
            config.setEnabledForExtensions(true);

            webServer.start();
            System.out.println("Xml-rpc server started");

        } catch (Exception e) {
            System.err.println("Error: " + e.getMessage());
        }
    }
}
package iisxmlrpcweather;

import org.w3c.dom.*;
import javax.xml.parsers.*;
import java.io.InputStream;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

public class WeatherServiceHandler {

    public List<String> getTemperature(String cityNamePart) {
        List<String> result = new ArrayList<>();

        try {
            URL url = new URL("https://vrijeme.hr/hrvatska_n.xml");
            InputStream stream = url.openStream();

            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            DocumentBuilder builder = factory.newDocumentBuilder();
            Document doc = builder.parse(stream);
            doc.getDocumentElement().normalize();

            NodeList gradList = doc.getElementsByTagName("Grad");

            for (int i = 0; i < gradList.getLength(); i++) {
                Element grad = (Element) gradList.item(i);
                String name = getSafeText(grad, "GradIme");
                Element podatci = (Element) grad.getElementsByTagName("Podatci").item(0);
                String temp = getSafeText(podatci, "Temp");

                if (name != null && temp != null && name.toLowerCase().contains(cityNamePart.toLowerCase())) {
                    result.add(name.trim() + ": " + temp.trim() + " °C");
                }
            }

        } catch (Exception e) {
            result.add("Error: " + e.getMessage());
        }

        return result;
    }

    private String getSafeText(Element parent, String tag) {
        if (parent == null) return null;
        NodeList nodes = parent.getElementsByTagName(tag);
        if (nodes.getLength() > 0 && nodes.item(0) != null) {
            return nodes.item(0).getTextContent();
        }
        return null;
    }
}
package iisjaxbvalidator;

import java.io.File;
import javax.xml.XMLConstants;
import javax.xml.bind.*;
import javax.xml.validation.Schema;
import javax.xml.validation.SchemaFactory;

public class XmlValidator {

    public static String validateAndGetMessage(String xmlPath, String xsdPath) {
        try {
            JAXBContext context = JAXBContext.newInstance(jaxbmodel.Profiles.class);
            Unmarshaller unmarshaller = context.createUnmarshaller();

            SchemaFactory schemaFactory = SchemaFactory.newInstance(XMLConstants.W3C_XML_SCHEMA_NS_URI);
            Schema schema = schemaFactory.newSchema(new File(xsdPath));
            unmarshaller.setSchema(schema);

            File xmlFile = new File(xmlPath);
            unmarshaller.unmarshal(xmlFile);

            return "Main xml is valid";
        } catch (JAXBException e) {
            Throwable cause = e.getLinkedException() != null ? e.getLinkedException() : e;
            return "JAXB validation failed:\n" + cause.getMessage();
        } catch (Exception e) {
            return "Eror:\n" + e.getMessage();
        }
    }
}

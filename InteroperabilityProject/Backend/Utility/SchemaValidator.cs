using System.Text;
using System.Xml;
using System.Xml.Schema;
using Commons.Xml.Relaxng;

namespace Backend.Utility;

public static class SchemaValidator
{
    public static List<string> ValidateXmlWithXsd(string xmlContent, string xsdPath)
    {
        var errors = new List<string>();

        var schemas = new XmlSchemaSet();
        schemas.Add("", xsdPath);

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemas
        };

        settings.ValidationEventHandler += (sender, e) =>
        {
            errors.Add($"Xsd: {e.Message}");
        };

        try
        {
            using var reader = XmlReader.Create(new StringReader(xmlContent), settings);
            while (reader.Read()) { }
        }
        catch (Exception ex)
        {
            errors.Add($"Xsd error: {ex.Message}");
        }

        return errors;
    }

    public static List<string> ValidateXmlWithRng(string xmlContent, string rngPath)
    {
        var errors = new List<string>();

        try
        {
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(true), leaveOpen: true))
            {
                writer.Write(xmlContent);
                writer.Flush();
                memoryStream.Position = 0;
            }

            using var readerWithEncoding = new StreamReader(memoryStream, Encoding.UTF8);
            using var xmlReader = new XmlTextReader(readerWithEncoding);
            using var rngReader = new XmlTextReader(rngPath);
            using var validatingReader = new RelaxngValidatingReader(xmlReader, rngReader);

            while (!validatingReader.EOF)
                validatingReader.Read();
        }
        catch (RelaxngException ex)
        {
            errors.Add($"Rng error: {ex.Message}");
        }

        return errors;
    }
}
using System.Xml.Serialization;

namespace DAL.DTOs;

public class ProfilePictureDto
{
    [XmlElement("Width")]
    public int? Width { get; set; }

    [XmlElement("Height")]
    public int? Height { get; set; }

    [XmlElement("Url")]
    public string? Url { get; set; }
}
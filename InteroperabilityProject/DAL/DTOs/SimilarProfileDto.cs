using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DAL.DTOs;

[XmlType("Profile")]
public class SimilarProfileDto
{
    [XmlElement("FirstName")]
    public string? FirstName { get; set; }

    [XmlElement("LastName")]
    public string? LastName { get; set; }

    [XmlElement("PublicIdentifier")]
    public string? PublicIdentifier { get; set; }

    [XmlElement("TitleV2")]
    public string? TitleV2 { get; set; }

    [XmlElement("TextActionTarget")]
    public string? TextActionTarget { get; set; }

    [XmlElement("Subtitle")]
    public string? Subtitle { get; set; }

    [XmlElement("SubtitleV2")]
    public string? SubtitleV2 { get; set; }

    [JsonProperty("creator")]
    [XmlIgnore]
    public bool? Creator { get; set; }

    [XmlElement("IsCreator")]
    public string? IsCreatorXml
    {
        get => Creator.HasValue ? Creator.Value.ToString().ToLower() : null;
        set { } 
    }

    public bool ShouldSerializeIsCreatorXml() => Creator.HasValue;

    [XmlArray("ProfilePictures")]
    [XmlArrayItem("ProfilePicture")]
    public List<ProfilePictureDto> ProfilePictures { get; set; } = new();
}
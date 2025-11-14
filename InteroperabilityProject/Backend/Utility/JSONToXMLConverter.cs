using DAL.DTOs;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Backend.Utility;

public static class JSONToXMLConverter
{
    public static void Convert(string json, string outputXmlPath)
    {
        var root = JObject.Parse(json);
        var newProfiles = root["response"]?.ToObject<List<SimilarProfileDto>>();

        if (newProfiles == null || !newProfiles.Any())
        {
            Console.WriteLine("No profiles found");
            return;
        }

        var serializer = new XmlSerializer(typeof(List<SimilarProfileDto>), new XmlRootAttribute("Profiles"));

        Directory.CreateDirectory(Path.GetDirectoryName(outputXmlPath)!);

        if (File.Exists(outputXmlPath))
        {
            var existingDoc = XDocument.Load(outputXmlPath);
            var existingIdentifiers = existingDoc
                .Root?
                .Elements("Profile")
                .Select(e => e.Element("PublicIdentifier")?.Value)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet() ?? new HashSet<string>();

            var newUniqueProfiles = newProfiles
                .Where(p => !string.IsNullOrWhiteSpace(p.PublicIdentifier) && !existingIdentifiers.Contains(p.PublicIdentifier))
                .ToList();

            if (!newUniqueProfiles.Any())
            {
                Console.WriteLine("No new profiles to add to main xml");
                return;
            }

            using var ms = new MemoryStream();
            serializer.Serialize(ms, newUniqueProfiles);
            ms.Position = 0;
            var temp = XDocument.Load(ms);

            var newElements = temp.Root?.Elements("Profile") ?? Enumerable.Empty<XElement>();
            existingDoc.Root?.Add(newElements);
            existingDoc.Save(outputXmlPath);
        }
        else
        {
            using var fs = new FileStream(outputXmlPath, FileMode.Create);
            serializer.Serialize(fs, newProfiles);
        }

        Console.WriteLine($"Main xml updated");
    }
}
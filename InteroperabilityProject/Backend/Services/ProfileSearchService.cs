using DAL.DTOs;
using Backend.Interfaces;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace Backend.Services
{
    public class ProfileSearchService : IProfileSearchService
    {
        private readonly IWebHostEnvironment _env;

        public ProfileSearchService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public List<SimilarProfileDto> SearchByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<SimilarProfileDto>();

            var filePath = Path.Combine("Data", "similar_profiles.xml");
            if (!File.Exists(filePath)) return new List<SimilarProfileDto>();

            var doc = new XPathDocument(filePath);
            var nav = doc.CreateNavigator();

            var lower = keyword.ToLower();

            var expr = $@"
            //Profile[
                contains(translate(FirstName, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{lower}') or
                contains(translate(LastName, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{lower}') or
                contains(translate(TitleV2, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{lower}')
            ]";

            var nodes = nav.Select(expr);

            var serializer = new XmlSerializer(typeof(SimilarProfileDto));
            var results = new List<SimilarProfileDto>();

            while (nodes.MoveNext())
            {
                var current = nodes.Current;
                if (current != null)
                {
                    using var reader = current.ReadSubtree();
                    if (serializer.Deserialize(reader) is SimilarProfileDto dto)
                        results.Add(dto);
                }
            }
            return results;
        }
    }
}
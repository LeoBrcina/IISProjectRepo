using Backend.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DAL.DTOs;
using DAL.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/entity")]
public class ProfilesController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly IisprojectDbContext _db;

    public ProfilesController(IHttpClientFactory httpClientFactory, IConfiguration config, IisprojectDbContext db)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _db = db;
    }

    [HttpPost("xsd-upload")]
    public async Task<IActionResult> UploadViaXsd([FromQuery] string profileUrl)
    {
        if (string.IsNullOrWhiteSpace(profileUrl) || !profileUrl.StartsWith("https://www.linkedin.com/in/"))
            return BadRequest("Invalid LinkedIn profile link");

        var rapidApiKey = _config["RapidAPI:Key"];
        var rapidApiHost = _config["RapidAPI:Host"];
        var requestUrl = $"https://{rapidApiHost}/similar_profiles?profileUrl={Uri.EscapeDataString(profileUrl)}";

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUrl),
            Headers =
            {
                { "x-rapidapi-key", rapidApiKey },
                { "x-rapidapi-host", rapidApiHost }
            }
        };

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{ex.Message}");
        }

        var json = await response.Content.ReadAsStringAsync();

        Directory.CreateDirectory("Data");
        var filePath = Path.Combine("Data", "similar_profiles.xml");

        JSONToXMLConverter.Convert(json, filePath);

        var xmlString = System.IO.File.ReadAllText(filePath);
        var xsdPath = Path.Combine("Schemas", "profile.xsd");
        var errors = SchemaValidator.ValidateXmlWithXsd(xmlString, xsdPath);
        if (errors.Any())
        {
            return BadRequest(new { message = "XML failed XSD validation.", errors });
        }

        var root = JObject.Parse(json);
        var profiles = root["response"]?.ToObject<List<DAL.DTOs.SimilarProfileDto>>();
        if (profiles == null || !profiles.Any())
            return BadRequest("No profiles");

        foreach (var dto in profiles)
        {
            if (string.IsNullOrWhiteSpace(dto.PublicIdentifier) || _db.Profiles.Any(x => x.PublicIdentifier == dto.PublicIdentifier))
                continue;

            var profile = new Profile
            {
                FirstName = dto.FirstName ?? "",
                LastName = dto.LastName ?? "",
                PublicIdentifier = dto.PublicIdentifier,
                TitleV2 = dto.TitleV2 ?? "",
                TextActionTarget = dto.TextActionTarget ?? "",
                Subtitle = dto.Subtitle,
                SubtitleV2 = dto.SubtitleV2,
                IsCreator = dto.Creator
            };

            if (dto.ProfilePictures != null)
            {
                foreach (var pic in dto.ProfilePictures)
                {
                    if (!string.IsNullOrWhiteSpace(pic.Url))
                    {
                        profile.ProfilePictures.Add(new ProfilePicture
                        {
                            Width = pic.Width ?? 0,
                            Height = pic.Height ?? 0,
                            Url = pic.Url
                        });
                    }
                }
            }

            _db.Profiles.Add(profile);
            await _db.SaveChangesAsync();
        }

        return Ok($"Validation successful");
    }

    [HttpPost("rng-upload")]
    public async Task<IActionResult> UploadViaRng([FromQuery] string profileUrl)
    {
        if (string.IsNullOrWhiteSpace(profileUrl) || !profileUrl.StartsWith("https://www.linkedin.com/in/"))
            return BadRequest("Invalid LinkedIn profile link");

        var rapidApiKey = _config["RapidAPI:Key"];
        var rapidApiHost = _config["RapidAPI:Host"];
        var requestUrl = $"https://{rapidApiHost}/similar_profiles?profileUrl={Uri.EscapeDataString(profileUrl)}";

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUrl),
            Headers =
        {
            { "x-rapidapi-key", rapidApiKey },
            { "x-rapidapi-host", rapidApiHost }
        }
        };

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{ex.Message}");
        }

        var json = await response.Content.ReadAsStringAsync();

        Directory.CreateDirectory("Data");
        var filePath = Path.Combine("Data", "similar_profiles.xml");

        JSONToXMLConverter.Convert(json, filePath);

        var xmlString = System.IO.File.ReadAllText(filePath);
        var rngPath = Path.Combine("Schemas", "profile.rng");
        var rngErrors = SchemaValidator.ValidateXmlWithRng(xmlString, rngPath);
        if (rngErrors.Any())
        {
            return BadRequest(new { message = "XML failed RNG validation.", errors = rngErrors });
        }

        var root = JObject.Parse(json);
        var profiles = root["response"]?.ToObject<List<DAL.DTOs.SimilarProfileDto>>();
        if (profiles == null || !profiles.Any())
            return BadRequest("No profiles");

        foreach (var dto in profiles)
        {
            if (string.IsNullOrWhiteSpace(dto.PublicIdentifier) || _db.Profiles.Any(x => x.PublicIdentifier == dto.PublicIdentifier))
                continue;

            var profile = new Profile
            {
                FirstName = dto.FirstName ?? "",
                LastName = dto.LastName ?? "",
                PublicIdentifier = dto.PublicIdentifier,
                TitleV2 = dto.TitleV2 ?? "",
                TextActionTarget = dto.TextActionTarget ?? "",
                Subtitle = dto.Subtitle,
                SubtitleV2 = dto.SubtitleV2,
                IsCreator = dto.Creator
            };

            if (dto.ProfilePictures != null)
            {
                foreach (var pic in dto.ProfilePictures)
                {
                    if (!string.IsNullOrWhiteSpace(pic.Url))
                    {
                        profile.ProfilePictures.Add(new ProfilePicture
                        {
                            Width = pic.Width ?? 0,
                            Height = pic.Height ?? 0,
                            Url = pic.Url
                        });
                    }
                }
            }

            _db.Profiles.Add(profile);
            await _db.SaveChangesAsync();
        }

        return Ok();
    }
}
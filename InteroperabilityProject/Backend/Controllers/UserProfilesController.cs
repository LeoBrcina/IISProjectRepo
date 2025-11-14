using DAL.Models;
using DAL.DTOs;
using Backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Backend.Controllers;

[ApiController]
[Route("api/userprofiles")]
[Authorize]
public class UserProfilesController : ControllerBase
{
    private readonly IisprojectDbContext _db;
    private readonly IWebHostEnvironment _env;

    public UserProfilesController(IisprojectDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var profiles = await _db.Profiles
            .Include(p => p.ProfilePictures)
            .ToListAsync();

        var results = profiles.Select(p => new SimilarProfileDto
        {
            FirstName = p.FirstName,
            LastName = p.LastName,
            PublicIdentifier = p.PublicIdentifier,
            TitleV2 = p.TitleV2,
            TextActionTarget = p.TextActionTarget,
            Subtitle = p.Subtitle,
            SubtitleV2 = p.SubtitleV2,
            Creator = p.IsCreator,
            ProfilePictures = p.ProfilePictures.Select(pic => new ProfilePictureDto
            {
                Width = pic.Width,
                Height = pic.Height,
                Url = pic.Url
            }).ToList()
        });

        return Ok(results);
    }

    [HttpGet("{publicIdentifier}")]
    public async Task<IActionResult> GetByPublicIdentifier(string publicIdentifier)
    {
        var profile = await _db.Profiles
            .Include(p => p.ProfilePictures)
            .FirstOrDefaultAsync(p => p.PublicIdentifier == publicIdentifier);

        if (profile == null)
            return NotFound("Profile not found");

        var result = new SimilarProfileDto
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            PublicIdentifier = profile.PublicIdentifier,
            TitleV2 = profile.TitleV2,
            TextActionTarget = profile.TextActionTarget,
            Subtitle = profile.Subtitle,
            SubtitleV2 = profile.SubtitleV2,
            Creator = profile.IsCreator,
            ProfilePictures = profile.ProfilePictures.Select(pic => new ProfilePictureDto
            {
                Width = pic.Width,
                Height = pic.Height,
                Url = pic.Url
            }).ToList()
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (_db.Profiles.Any(p =>
            p.FirstName == dto.FirstName &&
            p.LastName == dto.LastName &&
            p.TitleV2 == dto.TitleV2))
        {
            return Conflict("A profile with the same name already exists");
        }

        var username = User.FindFirstValue(ClaimTypes.Name);
        if (username == null)
            return Unauthorized();

        string publicId = GeneratePublicIdentifier(dto.FirstName, dto.LastName);

        if (_db.Profiles.Any(p => p.PublicIdentifier == publicId))
            return Conflict("A profile with the same ID already exists");

        string textActionTarget = GenerateTextActionTarget(publicId);

        var profile = new Profile
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PublicIdentifier = publicId,
            TitleV2 = dto.TitleV2,
            TextActionTarget = textActionTarget,
            Subtitle = dto.Subtitle,
            SubtitleV2 = dto.SubtitleV2,
            IsCreator = dto.IsCreator
        };

        if (dto.ProfilePictures != null)
        {
            foreach (var pic in dto.ProfilePictures)
            {
                profile.ProfilePictures.Add(new ProfilePicture
                {
                    Width = pic.Width,
                    Height = pic.Height,
                    Url = pic.Url
                });
            }
        }

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        var xmlDto = new SimilarProfileDto
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PublicIdentifier = publicId,
            TitleV2 = dto.TitleV2,
            TextActionTarget = textActionTarget,
            Subtitle = dto.Subtitle,
            SubtitleV2 = dto.SubtitleV2,
            Creator = dto.IsCreator,
            ProfilePictures = dto.ProfilePictures?.Select(p => new ProfilePictureDto
            {
                Width = p.Width,
                Height = p.Height,
                Url = p.Url
            }).ToList() ?? new()
        };

        var xmlPath = Path.Combine(_env.ContentRootPath, "Data", "similar_profiles.xml");
        JSONToXMLConverter.Convert(JsonConvert.SerializeObject(new { response = new[] { xmlDto } }), xmlPath);

        return Ok($"Main xml updated");
    }

    [HttpPut("{publicIdentifier}")]
    public async Task<IActionResult> Update(string publicIdentifier, [FromBody] CreateProfileDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = User.FindFirstValue(ClaimTypes.Name);
        if (username == null)
            return Unauthorized();

        var profile = await _db.Profiles
            .Include(p => p.ProfilePictures)
            .FirstOrDefaultAsync(p => p.PublicIdentifier == publicIdentifier);

        if (profile == null)
            return NotFound("Profile not found");

        profile.FirstName = dto.FirstName;
        profile.LastName = dto.LastName;
        profile.TitleV2 = dto.TitleV2;
        profile.TextActionTarget = dto.TextActionTarget;
        profile.Subtitle = dto.Subtitle;
        profile.SubtitleV2 = dto.SubtitleV2;
        profile.IsCreator = dto.IsCreator;

        profile.ProfilePictures.Clear();
        foreach (var pic in dto.ProfilePictures ?? new())
        {
            profile.ProfilePictures.Add(new ProfilePicture
            {
                Width = pic.Width,
                Height = pic.Height,
                Url = pic.Url
            });
        }

        await _db.SaveChangesAsync();

        var xmlDto = new SimilarProfileDto
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PublicIdentifier = publicIdentifier,
            TitleV2 = dto.TitleV2,
            TextActionTarget = dto.TextActionTarget,
            Subtitle = dto.Subtitle,
            SubtitleV2 = dto.SubtitleV2,
            Creator = dto.IsCreator,
            ProfilePictures = dto.ProfilePictures?.Select(p => new ProfilePictureDto
            {
                Width = p.Width,
                Height = p.Height,
                Url = p.Url
            }).ToList() ?? new()
        };

        var xmlPath = Path.Combine(_env.ContentRootPath, "Data", "similar_profiles.xml");
        if (System.IO.File.Exists(xmlPath))
        {
            var doc = XDocument.Load(xmlPath);

            var existing = doc.Root?
                .Elements("Profile")
                .FirstOrDefault(x => x.Element("PublicIdentifier")?.Value == publicIdentifier);

            if (existing != null)
            {
                existing.Remove();
            }

            var tempXml = new XDocument();
            var serializer = new XmlSerializer(typeof(List<SimilarProfileDto>), new XmlRootAttribute("Profiles"));
            using (var writer = tempXml.CreateWriter())
            {
                serializer.Serialize(writer, new List<SimilarProfileDto> { xmlDto });
            }

            var newElement = tempXml.Root?.Element("Profile");
            if (newElement != null)
            {
                doc.Root?.Add(newElement);
                doc.Save(xmlPath);
            }
        }

        return Ok($"Profile updated in main xml");
    }

    [HttpDelete("{publicIdentifier}")]
    public async Task<IActionResult> Delete(string publicIdentifier)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (username == null)
            return Unauthorized();

        var profile = await _db.Profiles
            .Include(p => p.ProfilePictures)
            .FirstOrDefaultAsync(p => p.PublicIdentifier == publicIdentifier);

        if (profile == null)
            return NotFound("Profile not found");

        _db.ProfilePictures.RemoveRange(profile.ProfilePictures);
        _db.Profiles.Remove(profile);
        await _db.SaveChangesAsync();

        var xmlPath = Path.Combine(_env.ContentRootPath, "Data", "similar_profiles.xml");
        if (System.IO.File.Exists(xmlPath))
        {
            var doc = XDocument.Load(xmlPath);
            var target = doc.Root?
                .Elements("Profile")
                .FirstOrDefault(x => x.Element("PublicIdentifier")?.Value == publicIdentifier);

            if (target != null)
            {
                target.Remove();
                doc.Save(xmlPath);
            }
        }

        return Ok($"Profile deleted from main xml");
    }

    private string GeneratePublicIdentifier(string firstName, string lastName)
    {
        string slugBase = $"{firstName}-{lastName}".ToLowerInvariant();
        slugBase = Regex.Replace(slugBase, @"[^a-z0-9\-]", "-");
        return $"{slugBase}-{Guid.NewGuid().ToString("N")[..8]}";
    }

    private string GenerateTextActionTarget(string publicIdentifier)
    {
        var miniUrn = $"urn%3Ali%3Afs_miniProfile%3A{Guid.NewGuid().ToString("N")[..16]}";
        return $"https://www.linkedin.com/in/{Uri.EscapeDataString(publicIdentifier)}?miniProfileUrn={miniUrn}";
    }
}
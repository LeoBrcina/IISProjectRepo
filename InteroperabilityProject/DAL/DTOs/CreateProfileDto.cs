using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs;

public class CreateProfileDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string TitleV2 { get; set; } = null!;

    public string? TextActionTarget { get; set; } = null!;

    public string? Subtitle { get; set; }

    public string? SubtitleV2 { get; set; }

    public bool? IsCreator { get; set; }

    public List<CreateProfilePictureDto>? ProfilePictures { get; set; }
}
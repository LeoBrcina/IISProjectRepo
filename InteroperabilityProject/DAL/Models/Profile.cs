using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Profile
{
    public int Id { get; set; }

    public string PublicIdentifier { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string TitleV2 { get; set; } = null!;

    public string TextActionTarget { get; set; } = null!;

    public string? Subtitle { get; set; }

    public string? SubtitleV2 { get; set; }

    public bool? IsCreator { get; set; }

    public virtual ICollection<ProfilePicture> ProfilePictures { get; set; } = new List<ProfilePicture>();
}

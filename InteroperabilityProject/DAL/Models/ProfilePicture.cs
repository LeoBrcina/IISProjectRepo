using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ProfilePicture
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string Url { get; set; } = null!;

    public virtual Profile Profile { get; set; } = null!;
}

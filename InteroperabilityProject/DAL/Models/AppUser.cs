using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class AppUser
{
    public int Iduser { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }
}

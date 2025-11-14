using Microsoft.AspNetCore.Mvc;
using DAL.DTOs;
using DAL.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IisprojectDbContext _db;
    private readonly JwtTokenGenerator _jwt;

    public AuthController(IisprojectDbContext db, JwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterRequestDto dto)
    {
        if (_db.AppUsers.Any(u => u.Username == dto.Username))
            return BadRequest("User already exists");

        var user = new AppUser
        {
            Username = dto.Username,
            Password = dto.Password
        };

        _db.AppUsers.Add(user);
        _db.SaveChanges();

        return Ok("Registered successfully");
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequestDto dto)
    {
        var user = _db.AppUsers.FirstOrDefault(u => u.Username == dto.Username);

        if (user == null || user.Password != dto.Password)
            return Unauthorized("Invalid username or password");

        var accessToken = _jwt.GenerateAccessToken(user.Username);
        var refreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _db.SaveChanges();

        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("Refresh")]
    public IActionResult Refresh([FromBody] RefreshRequestDto dto)
    {
        var user = _db.AppUsers.FirstOrDefault(u =>
            u.Username == dto.Username &&
            u.RefreshToken == dto.RefreshToken &&
            u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return Unauthorized("Invalid refresh token");

        var newAccessToken = _jwt.GenerateAccessToken(user.Username);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _db.SaveChanges();

        return Ok(new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}

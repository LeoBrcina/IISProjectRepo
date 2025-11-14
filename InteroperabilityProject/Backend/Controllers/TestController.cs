using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("secure")]
    [Authorize]
    public IActionResult SecureEndpoint()
    {
        return Ok();
    }

    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok();
    }
}
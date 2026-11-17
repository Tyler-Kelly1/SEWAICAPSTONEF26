using Backend.DTOs;
using Backend.Engines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthEngine _authEngine;

    public AuthController(IAuthEngine authEngine)
    {
        _authEngine = authEngine;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authEngine.RegisterAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authEngine.LoginAsync(dto, HttpContext);
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authEngine.LogoutAsync(HttpContext);
        return Ok(new { message = "Logged out successfully." });
    }

    [HttpGet("me")]
    public async Task<ActionResult<AuthUserDto>> GetCurrentUser()
    {
        var user = await _authEngine.GetCurrentUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "No active session." });
        }

        return Ok(user);
    }
}

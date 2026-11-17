using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserEngine _engine;
    private readonly IAuthEngine _authEngine;

    public UserController(IUserEngine engine, IAuthEngine authEngine)
    {
        _engine = engine;
        _authEngine = authEngine;
    }

    [AllowAnonymous]
    [HttpGet("exists/{userId}")]
    public async Task<ActionResult<UserCheckResultDto>> CheckUserExists(string userId)
    {
        var exists = await _engine.CheckUserExistsAsync(userId);
        return Ok(new UserCheckResultDto
        {
            UserId = userId,
            Exists = exists
        });
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUserById(string userId)
    {
        if (!_authEngine.ValidateUserAccess(User, userId))
        {
            return Forbid();
        }

        var user = await _engine.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        var user = await _engine.GetUserByIdAsync(authUserId);
        if (user == null)
        {
            return Ok(Enumerable.Empty<User>());
        }

        return Ok(new[] { user });
    }

    [HttpPost]
    public ActionResult CreateUser([FromBody] CreateUserDto dto)
    {
        return BadRequest(new { message = "Accounts must be created via the /api/auth/register endpoint with password authentication." });
    }
}

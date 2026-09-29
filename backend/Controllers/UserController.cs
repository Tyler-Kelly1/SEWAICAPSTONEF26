using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserEngine _engine;

    public UserController(IUserEngine engine)
    {
        _engine = engine;
    }

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
        var users = await _engine.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var created = await _engine.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { userId = created.UserId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

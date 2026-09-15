using Backend.DTOs;
using Backend.Engines;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionController : ControllerBase
{
    private readonly IWorkoutSessionEngine _engine;

    public WorkoutSessionController(IWorkoutSessionEngine engine)
    {
        _engine = engine;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _engine.GetUsersWithSessionsAsync();
        return Ok(users);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessions([FromQuery] string? userId = null)
    {
        var sessions = await _engine.GetSessionsAsync(userId);
        return Ok(sessions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SessionDto>> GetSessionById(int id)
    {
        var session = await _engine.GetSessionByIdAsync(id);
        if (session == null)
        {
            return NotFound($"Session with ID {id} not found.");
        }
        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession([FromBody] CreateSessionDto dto)
    {
        try
        {
            var session = await _engine.CreateSessionAsync(dto);
            return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, session);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

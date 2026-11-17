using Backend.DTOs;
using Backend.Engines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionController : ControllerBase
{
    private readonly IWorkoutSessionEngine _engine;
    private readonly IAuthEngine _authEngine;

    public WorkoutSessionController(IWorkoutSessionEngine engine, IAuthEngine authEngine)
    {
        _engine = engine;
        _authEngine = authEngine;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        var users = await _engine.GetUsersWithSessionsAsync(authUserId);
        return Ok(users);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessions([FromQuery] string? userId = null)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(userId) && !_authEngine.ValidateUserAccess(User, userId))
        {
            return Forbid();
        }

        var sessions = await _engine.GetSessionsAsync(authUserId);
        return Ok(sessions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SessionDto>> GetSessionById(int id)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        var session = await _engine.GetSessionByIdAsync(id, authUserId);
        if (session == null)
        {
            return NotFound($"Session with ID {id} not found.");
        }
        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession([FromBody] CreateSessionDto dto)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(dto.UserId) && !_authEngine.ValidateUserAccess(User, dto.UserId))
        {
            return Forbid();
        }

        dto.UserId = authUserId;

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

    [HttpPost("goal-sets")]
    public async Task<ActionResult<ExerciseGoalResponseDto>> GetGoalSets([FromBody] ExerciseGoalRequestDto dto)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(dto.UserId) && !_authEngine.ValidateUserAccess(User, dto.UserId))
        {
            return Forbid();
        }

        dto.UserId = authUserId;

        try
        {
            var result = await _engine.GetGoalSetsForExerciseAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("workout-templates")]
    public async Task<ActionResult<IEnumerable<WorkoutTemplateDto>>> GetWorkoutTemplates([FromQuery] string? userId = null)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(userId) && !_authEngine.ValidateUserAccess(User, userId))
        {
            return Forbid();
        }

        var templates = await _engine.GetWorkoutTemplatesAsync(authUserId);
        return Ok(templates);
    }

    [HttpGet("workout-templates/{id:int}")]
    public async Task<ActionResult<WorkoutTemplateDto>> GetWorkoutTemplateById(int id)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        var template = await _engine.GetWorkoutTemplateByIdAsync(id, authUserId);
        if (template == null)
        {
            return NotFound($"Workout template with ID {id} not found.");
        }
        return Ok(template);
    }

    [HttpPost("workout-templates")]
    public async Task<ActionResult<WorkoutTemplateDto>> CreateWorkoutTemplate([FromBody] CreateWorkoutTemplateDto dto)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(dto.UserId) && !_authEngine.ValidateUserAccess(User, dto.UserId))
        {
            return Forbid();
        }

        dto.UserId = authUserId;

        try
        {
            var template = await _engine.CreateWorkoutTemplateAsync(dto);
            return CreatedAtAction(nameof(GetWorkoutTemplateById), new { id = template.Id }, template);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("templates")]
    public async Task<ActionResult<IEnumerable<ExerciseTemplateDto>>> GetExerciseTemplates([FromQuery] string? userId = null)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(userId) && !_authEngine.ValidateUserAccess(User, userId))
        {
            return Forbid();
        }

        var templates = await _engine.GetExerciseTemplatesAsync(authUserId);
        return Ok(templates);
    }

    [HttpPost("templates")]
    public async Task<ActionResult<ExerciseTemplateDto>> CreateExerciseTemplate([FromBody] CreateExerciseTemplateDto dto)
    {
        var authUserId = _authEngine.GetAuthenticatedUserId(User);
        if (string.IsNullOrEmpty(authUserId))
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(dto.UserId) && !_authEngine.ValidateUserAccess(User, dto.UserId))
        {
            return Forbid();
        }

        dto.UserId = authUserId;

        try
        {
            var template = await _engine.CreateExerciseTemplateAsync(dto);
            return Ok(template);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

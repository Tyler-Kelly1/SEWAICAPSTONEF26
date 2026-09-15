using Backend.Controllers;
using Backend.DTOs;
using Backend.Engines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class WorkoutSessionControllerTests
{
    private FakeWorkoutSessionEngine _fakeEngine = null!;
    private WorkoutSessionController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeEngine = new FakeWorkoutSessionEngine();
        _controller = new WorkoutSessionController(_fakeEngine);
    }

    [TestMethod]
    public async Task GetUsers_ReturnsOkResultWithUsers()
    {
        // Act
        var result = await _controller.GetUsers();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var users = okResult.Value as IEnumerable<UserDto>;
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public async Task GetSessions_ReturnsOkResultWithSessions()
    {
        // Act
        var result = await _controller.GetSessions();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var sessions = okResult.Value as IEnumerable<SessionDto>;
        Assert.IsNotNull(sessions);
    }

    [TestMethod]
    public async Task CreateSession_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new CreateSessionDto
        {
            UserId = "user1",
            WorkoutName = "Morning Cardio",
            StartTime = now.AddHours(-1),
            EndTime = now
        };

        // Act
        var result = await _controller.CreateSession(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
    }
}

public class FakeWorkoutSessionEngine : IWorkoutSessionEngine
{
    private readonly List<SessionDto> _sessions = new();
    private readonly List<UserDto> _users = new()
    {
        new UserDto { UserId = "user1" }
    };

    public Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync()
    {
        return Task.FromResult<IEnumerable<UserDto>>(_users);
    }

    public Task<IEnumerable<SessionDto>> GetSessionsAsync(string? userId = null)
    {
        IEnumerable<SessionDto> result = _sessions;
        if (!string.IsNullOrEmpty(userId))
        {
            result = result.Where(s => s.UserId == userId);
        }
        return Task.FromResult(result);
    }

    public Task<SessionDto?> GetSessionByIdAsync(int id)
    {
        return Task.FromResult(_sessions.FirstOrDefault(s => s.Id == id));
    }

    public Task<SessionDto> CreateSessionAsync(CreateSessionDto dto)
    {
        var session = new SessionDto
        {
            Id = _sessions.Count + 1,
            UserId = dto.UserId,
            SessionDate = DateTime.UtcNow,
            Duration = dto.EndTime - dto.StartTime,
            ExecutedWorkout = new WorkoutDto
            {
                Id = 1,
                WorkoutName = dto.WorkoutName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            }
        };

        _sessions.Add(session);
        return Task.FromResult(session);
    }
}

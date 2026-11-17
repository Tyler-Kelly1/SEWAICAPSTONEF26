using System.Security.Claims;
using Backend.Controllers;
using Backend.DTOs;
using Backend.Engines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class WorkoutSessionControllerTests
{
    private FakeWorkoutSessionEngine _fakeEngine = null!;
    private FakeAuthEngine _fakeAuth = null!;
    private WorkoutSessionController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeEngine = new FakeWorkoutSessionEngine();
        _fakeAuth = new FakeAuthEngine();
        _controller = new WorkoutSessionController(_fakeEngine, _fakeAuth);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "user1"),
            new Claim(ClaimTypes.NameIdentifier, "user1")
        }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
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
    public async Task GetSessions_WithMismatchedUserId_ReturnsForbid()
    {
        // Act
        var result = await _controller.GetSessions("other_user");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task CreateSession_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateSessionDto
        {
            UserId = "user1",
            WorkoutName = "Push Day",
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow,
            Exercises = new List<CreateExerciseDto>
            {
                new CreateExerciseDto
                {
                    ExerciseName = "Bench Press",
                    Sets = new List<CreateSetDto>
                    {
                        new CreateSetDto { Weight = 135, Reps = 10 }
                    }
                }
            }
        };

        // Act
        var result = await _controller.CreateSession(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        var createdResult = (CreatedAtActionResult)result.Result!;
        var session = createdResult.Value as SessionDto;
        Assert.IsNotNull(session);
        Assert.AreEqual("user1", session.UserId);
    }

    [TestMethod]
    public async Task CreateSession_MismatchedUserId_ReturnsForbid()
    {
        // Arrange
        var dto = new CreateSessionDto
        {
            UserId = "spoofed_user",
            WorkoutName = "Push Day",
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow,
            Exercises = new List<CreateExerciseDto>()
        };

        // Act
        var result = await _controller.CreateSession(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task GetSessionById_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var createDto = new CreateSessionDto
        {
            UserId = "user1",
            WorkoutName = "Leg Day",
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow,
            Exercises = new List<CreateExerciseDto>()
        };
        await _controller.CreateSession(createDto);

        // Act
        var result = await _controller.GetSessionById(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var session = okResult.Value as SessionDto;
        Assert.IsNotNull(session);
        Assert.AreEqual(1, session.Id);
    }

    [TestMethod]
    public async Task GetSessionById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetSessionById(999);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetGoalSets_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var dto = new ExerciseGoalRequestDto
        {
            UserId = "user1",
            ExerciseName = "Barbell Bench Press",
            ExerciseTemplateId = 1,
            Sets = new List<CreateSetDto>
            {
                new CreateSetDto { Weight = 135, Reps = 10 }
            }
        };

        // Act
        var result = await _controller.GetGoalSets(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var response = okResult.Value as ExerciseGoalResponseDto;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.GoalSets.Count);
    }

    [TestMethod]
    public async Task GetGoalSets_MismatchedUserId_ReturnsForbid()
    {
        // Arrange
        var dto = new ExerciseGoalRequestDto
        {
            UserId = "other_user",
            ExerciseName = "Barbell Bench Press"
        };

        // Act
        var result = await _controller.GetGoalSets(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task GetWorkoutTemplates_ReturnsOkResult()
    {
        // Act
        var result = await _controller.GetWorkoutTemplates();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var templates = okResult.Value as IEnumerable<WorkoutTemplateDto>;
        Assert.IsNotNull(templates);
    }

    [TestMethod]
    public async Task CreateWorkoutTemplate_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateWorkoutTemplateDto
        {
            UserId = "user1",
            Workout_Name = "Pull Day Power",
            ExerciseTemplates = new List<CreateExerciseTemplateDto>()
        };

        // Act
        var result = await _controller.CreateWorkoutTemplate(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public async Task GetExerciseTemplates_ReturnsOkResult()
    {
        // Act
        var result = await _controller.GetExerciseTemplates();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task CreateExerciseTemplate_ValidDto_ReturnsOkResult()
    {
        // Arrange
        var dto = new CreateExerciseTemplateDto
        {
            Exercise_Name = "Overhead Press",
            Min_Set = 3,
            Max_Set = 5
        };

        // Act
        var result = await _controller.CreateExerciseTemplate(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }
}

public class FakeWorkoutSessionEngine : IWorkoutSessionEngine
{
    private readonly List<SessionDto> _sessions = new();
    private readonly List<WorkoutTemplateDto> _workoutTemplates = new()
    {
        new WorkoutTemplateDto { Id = 1, Workout_Name = "Upper Body Power Template" }
    };
    private readonly List<ExerciseTemplateDto> _exerciseTemplates = new()
    {
        new ExerciseTemplateDto { Id = 1, Exercise_Name = "Barbell Bench Press" }
    };
    private readonly List<UserDto> _users = new()
    {
        new UserDto { UserId = "user1" }
    };

    public Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync(string? userId = null)
    {
        IEnumerable<UserDto> result = _users;
        if (!string.IsNullOrEmpty(userId))
        {
            result = result.Where(u => u.UserId == userId);
        }
        return Task.FromResult(result);
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

    public Task<SessionDto?> GetSessionByIdAsync(int id, string? userId = null)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == id);
        if (session != null && !string.IsNullOrEmpty(userId) && session.UserId != userId)
        {
            return Task.FromResult<SessionDto?>(null);
        }
        return Task.FromResult(session);
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

    public Task<ExerciseGoalResponseDto> GetGoalSetsForExerciseAsync(ExerciseGoalRequestDto dto)
    {
        var response = new ExerciseGoalResponseDto
        {
            ExerciseName = dto.ExerciseName,
            ExerciseTemplateId = dto.ExerciseTemplateId,
            GoalSets = dto.Sets?.Select((s, index) => new GoalSetDto
            {
                SetNumber = index + 1,
                Weight = s.Weight + 5,
                Reps = s.Reps
            }).ToList() ?? new List<GoalSetDto>()
        };
        return Task.FromResult(response);
    }

    public Task<IEnumerable<WorkoutTemplateDto>> GetWorkoutTemplatesAsync(string? userId = null)
    {
        return Task.FromResult<IEnumerable<WorkoutTemplateDto>>(_workoutTemplates);
    }

    public Task<WorkoutTemplateDto?> GetWorkoutTemplateByIdAsync(int id, string? userId = null)
    {
        return Task.FromResult(_workoutTemplates.FirstOrDefault(t => t.Id == id));
    }

    public Task<WorkoutTemplateDto> CreateWorkoutTemplateAsync(CreateWorkoutTemplateDto dto)
    {
        var template = new WorkoutTemplateDto
        {
            Id = _workoutTemplates.Count + 1,
            Workout_Name = dto.Workout_Name
        };
        _workoutTemplates.Add(template);
        return Task.FromResult(template);
    }

    public Task<IEnumerable<ExerciseTemplateDto>> GetExerciseTemplatesAsync(string? userId = null)
    {
        return Task.FromResult<IEnumerable<ExerciseTemplateDto>>(_exerciseTemplates);
    }

    public Task<ExerciseTemplateDto> CreateExerciseTemplateAsync(CreateExerciseTemplateDto dto)
    {
        var template = new ExerciseTemplateDto
        {
            Id = _exerciseTemplates.Count + 1,
            Exercise_Name = dto.Exercise_Name,
            Min_Set = dto.Min_Set,
            Max_Set = dto.Max_Set
        };
        _exerciseTemplates.Add(template);
        return Task.FromResult(template);
    }
}

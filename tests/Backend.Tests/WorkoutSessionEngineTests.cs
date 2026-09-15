using Backend.Data;
using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class WorkoutSessionEngineTests
{
    private AppDbContext _context = null!;
    private WorkoutSessionEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _engine = new WorkoutSessionEngine(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public async Task GetUsersWithSessionsAsync_ReturnsSeededUsers()
    {
        // Arrange
        await DbInitializer.SeedAsync(_context);

        // Act
        var users = await _engine.GetUsersWithSessionsAsync();

        // Assert
        Assert.IsNotNull(users);
        var userList = users.ToList();
        Assert.IsTrue(userList.Count >= 2);
        Assert.IsTrue(userList.Any(u => u.UserId == "tyler_dev"));
    }

    [TestMethod]
    public async Task GetSessionsAsync_ReturnsAllSessions()
    {
        // Arrange
        await DbInitializer.SeedAsync(_context);

        // Act
        var sessions = await _engine.GetSessionsAsync();

        // Assert
        Assert.IsNotNull(sessions);
        var sessionList = sessions.ToList();
        Assert.AreEqual(3, sessionList.Count);
    }

    [TestMethod]
    public async Task CreateSessionAsync_ValidDto_CreatesAndReturnsSession()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new CreateSessionDto
        {
            UserId = "test_user",
            WorkoutName = "Test Power Workout",
            StartTime = now.AddHours(-1),
            EndTime = now,
            Exercises = new List<CreateExerciseDto>
            {
                new CreateExerciseDto
                {
                    ExerciseName = "Incline Press",
                    Sets = new List<CreateSetDto>
                    {
                        new CreateSetDto { Weight = 135, Reps = 10 }
                    }
                }
            }
        };

        // Act
        var result = await _engine.CreateSessionAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test_user", result.UserId);
        Assert.AreEqual("Test Power Workout", result.ExecutedWorkout.WorkoutName);
        Assert.AreEqual(1, result.ExecutedWorkout.Exercises.Count);
        Assert.AreEqual("Incline Press", result.ExecutedWorkout.Exercises[0].ExerciseName);
        Assert.AreEqual(135, result.ExecutedWorkout.Exercises[0].Sets[0].Weight);
    }

    [TestMethod]
    public async Task CreateSessionAsync_EndTimeBeforeStartTime_ThrowsArgumentException()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new CreateSessionDto
        {
            UserId = "test_user",
            WorkoutName = "Invalid Workout",
            StartTime = now,
            EndTime = now.AddHours(-1),
            Exercises = new List<CreateExerciseDto>()
        };

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await _engine.CreateSessionAsync(dto);
        });
    }
}

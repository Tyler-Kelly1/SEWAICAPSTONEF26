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
        _engine = new WorkoutSessionEngine(_context, new OverloadEngine(_context));
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

    [TestMethod]
    public async Task GetGoalSetsForExerciseAsync_SeededExercise_CalculatesGoalSetsCorrectly()
    {
        // Arrange
        await DbInitializer.SeedAsync(_context);
        var dto = new ExerciseGoalRequestDto
        {
            ExerciseName = "Barbell Bench Press",
            Sets = new List<CreateSetDto>
            {
                new CreateSetDto { Weight = 185, Reps = 8 },  // Hypertrophy (min=6, max=10): Volume Overload -> 185, 9
                new CreateSetDto { Weight = 205, Reps = 8 },  // Failure (min=4, max=8): Ceiling Overload -> 215, 4
                new CreateSetDto { Weight = 225, Reps = 3 }   // Failure (min=4, max=8): Underload -> 214, 4
            }
        };

        // Act
        var result = await _engine.GetGoalSetsForExerciseAsync(dto);

        // Assert: Uses true DB previous values (185x8, 205x6, 225x4)
        Assert.IsNotNull(result);
        Assert.AreEqual("Barbell Bench Press", result.ExerciseName);
        Assert.AreEqual(3, result.GoalSets.Count);

        // Set 1: DB set (185 lbs, 8 reps) with setTempHypertrophy (min=6, max=10) -> Volume overload -> 185, 9
        Assert.AreEqual(1, result.GoalSets[0].SetNumber);
        Assert.AreEqual(185, result.GoalSets[0].Weight);
        Assert.AreEqual(9, result.GoalSets[0].Reps);

        // Set 2: DB set (205 lbs, 6 reps) with setTemp (min=4, max=8) -> Volume overload -> 205, 7
        Assert.AreEqual(2, result.GoalSets[1].SetNumber);
        Assert.AreEqual(205, result.GoalSets[1].Weight);
        Assert.AreEqual(7, result.GoalSets[1].Reps);

        // Set 3: DB set (225 lbs, 4 reps) with setTemp (min=4, max=8) -> Volume overload -> 225, 5
        Assert.AreEqual(3, result.GoalSets[2].SetNumber);
        Assert.AreEqual(225, result.GoalSets[2].Weight);
        Assert.AreEqual(5, result.GoalSets[2].Reps);
    }

    [TestMethod]
    public async Task GetGoalSetsForExerciseAsync_UnseededExercise_UsesFallbackTemplate()
    {
        // Arrange
        var dto = new ExerciseGoalRequestDto
        {
            ExerciseName = "Custom Overhead Extension",
            Sets = new List<CreateSetDto>
            {
                new CreateSetDto { Weight = 50, Reps = 10 } // Default template max reps = 10 -> Ceiling -> 50*1.05=52.5 (rounded to 52), 6 reps
            }
        };

        // Act
        var result = await _engine.GetGoalSetsForExerciseAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Custom Overhead Extension", result.ExerciseName);
        Assert.AreEqual(1, result.GoalSets.Count);
        Assert.AreEqual(52, result.GoalSets[0].Weight);
        Assert.AreEqual(6, result.GoalSets[0].Reps);
    }
}

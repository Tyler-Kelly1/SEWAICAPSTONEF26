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
    public async Task GetSessionsAsync_ReturnsAllSessionsForUser()
    {
        // Arrange
        await DbInitializer.SeedAsync(_context);

        // Act
        var sessions = await _engine.GetSessionsAsync("tyler_dev");

        // Assert
        Assert.IsNotNull(sessions);
        var sessionList = sessions.ToList();
        Assert.IsTrue(sessionList.Count >= 2);
    }

    [TestMethod]
    public async Task CreateSessionAsync_ValidDto_CreatesAndReturnsSession()
    {
        // Arrange
        _context.Users.Add(new ApplicationUser { Id = "test_user", UserName = "test_user" });
        await _context.SaveChangesAsync();

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
            UserId = "tyler_dev",
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

    [TestMethod]
    public async Task GetWorkoutTemplatesAsync_ReturnsSeededWorkoutTemplates()
    {
        // Arrange
        await DbInitializer.SeedAsync(_context);

        // Act
        var templates = await _engine.GetWorkoutTemplatesAsync("tyler_dev");

        // Assert
        Assert.IsNotNull(templates);
        var templateList = templates.ToList();
        Assert.IsTrue(templateList.Count >= 3);
        Assert.IsTrue(templateList.Any(t => t.Workout_Name.Contains("Upper Body")));
        Assert.IsTrue(templateList.Any(t => t.Workout_Name.Contains("Lower Body")));
        Assert.IsTrue(templateList.Any(t => t.Workout_Name.Contains("Push Day")));
    }

    [TestMethod]
    public async Task CreateWorkoutTemplateAsync_ValidDto_CreatesAndReturnsWorkoutTemplate()
    {
        // Arrange
        var dto = new CreateWorkoutTemplateDto
        {
            UserId = "tyler_dev",
            Workout_Name = "Pull Day Power",
            ExerciseTemplates = new List<CreateExerciseTemplateDto>
            {
                new CreateExerciseTemplateDto
                {
                    Exercise_Name = "Lat Pulldown",
                    Min_Set = 3,
                    Max_Set = 4,
                    SetTemplates = new List<CreateSetTemplateDto>
                    {
                        new CreateSetTemplateDto { Min_Reps = 8, Max_Reps = 12, Failure_Set = false }
                    }
                }
            }
        };

        // Act
        var result = await _engine.CreateWorkoutTemplateAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Pull Day Power", result.Workout_Name);
        Assert.AreEqual(1, result.ExerciseTemplates.Count);
        Assert.AreEqual("Lat Pulldown", result.ExerciseTemplates[0].Exercise_Name);
        Assert.AreEqual(1, result.ExerciseTemplates[0].SetTemplates.Count);
    }

    [TestMethod]
    public async Task GetExerciseTemplatesAsync_ReturnsSeededExerciseTemplates()
    {
        // Arrange
        await DbInitializer.SeedAsync(_context);

        // Act
        var templates = await _engine.GetExerciseTemplatesAsync("tyler_dev");

        // Assert
        Assert.IsNotNull(templates);
        var templateList = templates.ToList();
        Assert.IsTrue(templateList.Count >= 6);
        Assert.IsTrue(templateList.Any(e => e.Exercise_Name == "Barbell Bench Press"));
        Assert.IsTrue(templateList.Any(e => e.Exercise_Name == "Barbell Back Squat"));
    }

    [TestMethod]
    public async Task CreateExerciseTemplateAsync_ValidDto_CreatesAndReturnsExerciseTemplate()
    {
        // Arrange
        var dto = new CreateExerciseTemplateDto
        {
            Exercise_Name = "Leg Extension",
            Min_Set = 3,
            Max_Set = 4,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<CreateSetTemplateDto>
            {
                new CreateSetTemplateDto { Min_Reps = 10, Max_Reps = 15, Failure_Set = false }
            }
        };

        // Act
        var result = await _engine.CreateExerciseTemplateAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Leg Extension", result.Exercise_Name);
        Assert.AreEqual(1, result.SetTemplates.Count);
        Assert.AreEqual(10, result.SetTemplates[0].Min_Reps);
    }
}

using Backend.Data;
using Backend.Engines;
using Backend.Models;
using Backend.Models.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class OverloadEngineTests
{
    private AppDbContext _context = null!;
    private OverloadEngine _engine = null!;
    private Exercise_Template _exerciseTemplate = null!;
    private Set_Template _setTemplate = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _engine = new OverloadEngine(_context);
        _exerciseTemplate = new Exercise_Template
        {
            Exercise_Name = "Bench Press",
            Min_Set = 3,
            Max_Set = 5,
            Weight_Step = 0.05m, // 5%
            Volume_Step = 1
        };
        _setTemplate = new Set_Template
        {
            Min_Reps = 6,
            Max_Reps = 10,
            Failure_Set = false
        };
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void GetGoalSet_OverloadCeiling_IncreasesWeightAndResetsToMinReps()
    {
        // Arrange: previous reps >= maxReps (10 reps >= 10)
        var previousSet = new Set_Value { Weight = 200, Reps = 10 };

        // Act
        var goalSet = _engine.GetGoalSet(_exerciseTemplate, _setTemplate, previousSet);

        // Assert: 200 * (1 + 0.05) = 210, reps = 6 (Min_Reps)
        Assert.AreEqual(210, goalSet.Weight);
        Assert.AreEqual(6, goalSet.Reps);
    }

    [TestMethod]
    public void GetGoalSet_OverloadVolume_IncreasesRepsByVolumeStep()
    {
        // Arrange: previous reps between Min_Reps and Max_Reps (7 reps > 6)
        var previousSet = new Set_Value { Weight = 200, Reps = 7 };

        // Act
        var goalSet = _engine.GetGoalSet(_exerciseTemplate, _setTemplate, previousSet);

        // Assert: Weight stays 200, reps = 7 + 1 = 8
        Assert.AreEqual(200, goalSet.Weight);
        Assert.AreEqual(8, goalSet.Reps);
    }

    [TestMethod]
    public void GetGoalSet_Underload_ReducesWeightAndResetsToMinReps()
    {
        // Arrange: previous reps < Min_Reps (4 reps < 6)
        var previousSet = new Set_Value { Weight = 200, Reps = 4 };

        // Act
        var goalSet = _engine.GetGoalSet(_exerciseTemplate, _setTemplate, previousSet);

        // Assert: 200 * (1 - 0.05) = 190, reps = 6 (Min_Reps)
        Assert.AreEqual(190, goalSet.Weight);
        Assert.AreEqual(6, goalSet.Reps);
    }

    [TestMethod]
    public void GetGoalSet_DirectParameters_CalculatesCorrectly()
    {
        // Act: Overload Ceiling with 100lbs, 12 reps, min=8, max=12, weightStep=0.10, volStep=2
        var goalSet = _engine.GetGoalSet(
            previousReps: 12,
            previousWeight: 100,
            minReps: 8,
            maxReps: 12,
            weightStep: 0.10m,
            volumeStep: 2
        );

        // Assert: 100 * 1.10 = 110, reps = 8
        Assert.AreEqual(110, goalSet.Weight);
        Assert.AreEqual(8, goalSet.Reps);
    }

    [TestMethod]
    public async Task GetPreviousSetValueAsync_ExistingSessionInDb_ReturnsCorrectPreviousSetValue()
    {
        // Arrange: seed DB with user, session, workout, exercise, set
        var user = new User { UserId = "user1" };
        var workout = new Workout
        {
            WorkoutName = "Chest Day",
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow.AddDays(-1).AddHours(1),
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Bench Press",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 225, Reps = 10 },
                        new Set { Weight = 235, Reps = 8 }
                    }
                }
            }
        };

        var session = new Session
        {
            UserId = "user1",
            ExecutedWorkout = workout,
            SessionDate = DateTime.UtcNow.AddDays(-1)
        };

        _context.Users.Add(user);
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        // Act: fetch previous set for Bench Press set index 0 and 1
        var set0 = await _engine.GetPreviousSetValueAsync(null, "Bench Press", 0, "user1");
        var set1 = await _engine.GetPreviousSetValueAsync(null, "Bench Press", 1, "user1");
        var set2 = await _engine.GetPreviousSetValueAsync(null, "Bench Press", 2, "user1");

        // Assert
        Assert.IsNotNull(set0);
        Assert.AreEqual(225, set0.Weight);
        Assert.AreEqual(10, set0.Reps);

        Assert.IsNotNull(set1);
        Assert.AreEqual(235, set1.Weight);
        Assert.AreEqual(8, set1.Reps);

        Assert.IsNull(set2);
    }
}

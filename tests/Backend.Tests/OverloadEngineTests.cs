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
        var user = new ApplicationUser { Id = "user1", UserName = "user1", Email = "user1@example.com" };
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

    [TestMethod]
    public async Task GetPreviousSetValueAsync_NullOrEmptyUserId_ReturnsNull()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user1", UserName = "user1" };
        var workout = new Workout
        {
            WorkoutName = "Chest Day",
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow,
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Bench Press",
                    Sets = new List<Set> { new Set { Weight = 225, Reps = 10 } }
                }
            }
        };
        var session = new Session { UserId = "user1", ExecutedWorkout = workout };
        _context.Users.Add(user);
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        // Act: call with null and empty userId
        var setWithNull = await _engine.GetPreviousSetValueAsync(null, "Bench Press", 0, null);
        var setWithEmpty = await _engine.GetPreviousSetValueAsync(null, "Bench Press", 0, "");

        // Assert: both MUST return null to prevent cross-user leakage
        Assert.IsNull(setWithNull);
        Assert.IsNull(setWithEmpty);
    }

    [TestMethod]
    public async Task GetPreviousSetValueAsync_MultipleUsers_DoesNotCrossContaminate()
    {
        // Arrange: User 1 benched 300 lbs, User 2 benched 150 lbs
        var user1 = new ApplicationUser { Id = "user1", UserName = "user1" };
        var user2 = new ApplicationUser { Id = "user2", UserName = "user2" };
        var workout1 = new Workout
        {
            WorkoutName = "User 1 Workout",
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow,
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Bench Press",
                    Sets = new List<Set> { new Set { Weight = 300, Reps = 10 } }
                }
            }
        };
        var workout2 = new Workout
        {
            WorkoutName = "User 2 Workout",
            StartTime = DateTime.UtcNow.AddMinutes(-10), // More recent!
            EndTime = DateTime.UtcNow,
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Bench Press",
                    Sets = new List<Set> { new Set { Weight = 150, Reps = 10 } }
                }
            }
        };

        _context.Users.AddRange(user1, user2);
        _context.Sessions.AddRange(
            new Session { UserId = "user1", ExecutedWorkout = workout1 },
            new Session { UserId = "user2", ExecutedWorkout = workout2 }
        );
        await _context.SaveChangesAsync();

        // Act: query for User 1
        var user1Set = await _engine.GetPreviousSetValueAsync(null, "Bench Press", 0, "user1");

        // Assert: Must return User 1's 300 lbs, NOT User 2's more recent 150 lbs
        Assert.IsNotNull(user1Set);
        Assert.AreEqual(300, user1Set.Weight);
    }
}

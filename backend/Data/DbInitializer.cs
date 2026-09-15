using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Ensure database tables exist
        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync())
        {
            return; // DB has already been seeded
        }

        var now = DateTime.UtcNow;

        var user1 = new User { UserId = "tyler_dev" };
        var user2 = new User { UserId = "alex_fit" };

        context.Users.AddRange(user1, user2);

        // Workout 1 for Tyler
        var workout1 = new Workout
        {
            WorkoutName = "Upper Body Power",
            StartTime = now.AddDays(-2).AddHours(-2),
            EndTime = now.AddDays(-2).AddHours(-1).AddMinutes(-15), // 1 hour 45 min
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Barbell Bench Press",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 185, Reps = 8 },
                        new Set { Weight = 205, Reps = 6 },
                        new Set { Weight = 225, Reps = 4 }
                    }
                },
                new Exercise
                {
                    ExerciseName = "Bent Over Row",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 135, Reps = 10 },
                        new Set { Weight = 155, Reps = 8 },
                        new Set { Weight = 175, Reps = 6 }
                    }
                }
            }
        };

        // Workout 2 for Tyler
        var workout2 = new Workout
        {
            WorkoutName = "Leg Day Blitz",
            StartTime = now.AddDays(-1).AddHours(-3),
            EndTime = now.AddDays(-1).AddHours(-2), // 1 hour
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Barbell Back Squat",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 225, Reps = 8 },
                        new Set { Weight = 275, Reps = 5 },
                        new Set { Weight = 315, Reps = 3 }
                    }
                },
                new Exercise
                {
                    ExerciseName = "Romanian Deadlift",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 185, Reps = 10 },
                        new Set { Weight = 205, Reps = 8 }
                    }
                }
            }
        };

        // Workout 3 for Alex
        var workout3 = new Workout
        {
            WorkoutName = "Full Body Conditioning",
            StartTime = now.AddHours(-4),
            EndTime = now.AddHours(-3).AddMinutes(-10), // 50 min
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Overhead Press",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 95, Reps = 10 },
                        new Set { Weight = 115, Reps = 8 }
                    }
                },
                new Exercise
                {
                    ExerciseName = "Pull-Ups",
                    Sets = new List<Set>
                    {
                        new Set { Weight = 0, Reps = 12 },
                        new Set { Weight = 0, Reps = 10 },
                        new Set { Weight = 0, Reps = 8 }
                    }
                }
            }
        };

        var session1 = new Session
        {
            UserId = user1.UserId,
            ExecutedWorkout = workout1,
            SessionDate = now.AddDays(-2)
        };

        var session2 = new Session
        {
            UserId = user1.UserId,
            ExecutedWorkout = workout2,
            SessionDate = now.AddDays(-1)
        };

        var session3 = new Session
        {
            UserId = user2.UserId,
            ExecutedWorkout = workout3,
            SessionDate = now
        };

        context.Sessions.AddRange(session1, session2, session3);
        await context.SaveChangesAsync();
    }
}

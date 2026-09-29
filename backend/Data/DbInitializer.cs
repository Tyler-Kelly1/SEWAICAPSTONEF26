using Backend.Models;
using Backend.Models.Templates;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Ensure database tables exist
        await context.Database.EnsureCreatedAsync();

        if (await context.WorkoutTemplates.AnyAsync())
        {
            return; // DB has already been seeded
        }

        var now = DateTime.UtcNow;

        Func<Set_Template> createHypertrophySet = () => new Set_Template { Failure_Set = false, Min_Reps = 6, Max_Reps = 10 };
        Func<Set_Template> createFailureSet = () => new Set_Template { Failure_Set = true, Min_Reps = 4, Max_Reps = 8 };
        Func<Set_Template> createStrengthSet = () => new Set_Template { Failure_Set = false, Min_Reps = 5, Max_Reps = 8 };
        Func<Set_Template> createEnduranceSet = () => new Set_Template { Failure_Set = false, Min_Reps = 8, Max_Reps = 12 };

        var exTempBench = new Exercise_Template
        {
            Exercise_Name = "Barbell Bench Press",
            Min_Set = 3,
            Max_Set = 5,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createHypertrophySet(), createHypertrophySet(), createFailureSet() }
        };

        var exTempRow = new Exercise_Template
        {
            Exercise_Name = "Bent Over Row",
            Min_Set = 3,
            Max_Set = 4,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createHypertrophySet(), createHypertrophySet() }
        };

        var exTempSquat = new Exercise_Template
        {
            Exercise_Name = "Barbell Back Squat",
            Min_Set = 3,
            Max_Set = 5,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createStrengthSet(), createStrengthSet(), createFailureSet() }
        };

        var exTempRDL = new Exercise_Template
        {
            Exercise_Name = "Romanian Deadlift",
            Min_Set = 3,
            Max_Set = 4,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createEnduranceSet(), createEnduranceSet() }
        };

        var exTempOHP = new Exercise_Template
        {
            Exercise_Name = "Overhead Press",
            Min_Set = 3,
            Max_Set = 4,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createHypertrophySet(), createHypertrophySet() }
        };

        var exTempInclineDumbbell = new Exercise_Template
        {
            Exercise_Name = "Incline Dumbbell Press",
            Min_Set = 3,
            Max_Set = 4,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createEnduranceSet(), createEnduranceSet() }
        };

        var exTempPushBench = new Exercise_Template
        {
            Exercise_Name = "Barbell Bench Press",
            Min_Set = 3,
            Max_Set = 5,
            Weight_Step = 0.05m,
            Volume_Step = 1,
            SetTemplates = new List<Set_Template> { createHypertrophySet(), createHypertrophySet(), createFailureSet() }
        };

        var workoutTempUpper = new Workout_Template
        {
            Workout_Name = "Upper Body Template",
            ExerciseTemplates = new List<Exercise_Template> { exTempBench, exTempRow }
        };

        var workoutTempLower = new Workout_Template
        {
            Workout_Name = "Lower Body Power Template",
            ExerciseTemplates = new List<Exercise_Template> { exTempSquat, exTempRDL }
        };

        var workoutTempPush = new Workout_Template
        {
            Workout_Name = "Push Day Hypertrophy",
            ExerciseTemplates = new List<Exercise_Template> { exTempPushBench, exTempOHP, exTempInclineDumbbell }
        };

        context.WorkoutTemplates.AddRange(workoutTempUpper, workoutTempLower, workoutTempPush);

        var user1 = await context.Users.FindAsync("tyler_dev");
        if (user1 == null)
        {
            user1 = new User { UserId = "tyler_dev" };
            context.Users.Add(user1);
        }

        var user2 = await context.Users.FindAsync("alex_fit");
        if (user2 == null)
        {
            user2 = new User { UserId = "alex_fit" };
            context.Users.Add(user2);
        }

        // Workout 1 for Tyler
        var workout1 = new Workout
        {
            WorkoutName = "Upper Body Power",
            StartTime = now.AddDays(-2).AddHours(-2),
            EndTime = now.AddDays(-2).AddHours(-1).AddMinutes(-15),
            Template = workoutTempUpper,
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Barbell Bench Press",
                    Template = exTempBench,
                    Sets = new List<Set>
                    {
                        new Set { Weight = 185, Reps = 8, Template = exTempBench.SetTemplates[0] },
                        new Set { Weight = 205, Reps = 6, Template = exTempBench.SetTemplates[1] },
                        new Set { Weight = 225, Reps = 4, Template = exTempBench.SetTemplates[2] }
                    }
                },
                new Exercise
                {
                    ExerciseName = "Bent Over Row",
                    Template = exTempRow,
                    Sets = new List<Set>
                    {
                        new Set { Weight = 135, Reps = 10, Template = exTempRow.SetTemplates[0] },
                        new Set { Weight = 155, Reps = 8, Template = exTempRow.SetTemplates[1] },
                        new Set { Weight = 175, Reps = 6, Template = exTempRow.SetTemplates[1] }
                    }
                }
            }
        };

        // Workout 2 for Tyler
        var workout2 = new Workout
        {
            WorkoutName = "Leg Day Blitz",
            StartTime = now.AddDays(-1).AddHours(-3),
            EndTime = now.AddDays(-1).AddHours(-2),
            Template = workoutTempLower,
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Barbell Back Squat",
                    Template = exTempSquat,
                    Sets = new List<Set>
                    {
                        new Set { Weight = 225, Reps = 8, Template = exTempSquat.SetTemplates[0] },
                        new Set { Weight = 275, Reps = 5, Template = exTempSquat.SetTemplates[1] },
                        new Set { Weight = 315, Reps = 3, Template = exTempSquat.SetTemplates[2] }
                    }
                },
                new Exercise
                {
                    ExerciseName = "Romanian Deadlift",
                    Template = exTempRDL,
                    Sets = new List<Set>
                    {
                        new Set { Weight = 185, Reps = 10, Template = exTempRDL.SetTemplates[0] },
                        new Set { Weight = 205, Reps = 8, Template = exTempRDL.SetTemplates[1] }
                    }
                }
            }
        };

        // Workout 3 for Alex
        var workout3 = new Workout
        {
            WorkoutName = "Full Body Conditioning",
            StartTime = now.AddHours(-4),
            EndTime = now.AddHours(-3).AddMinutes(-10),
            Template = workoutTempPush,
            Exercises = new List<Exercise>
            {
                new Exercise
                {
                    ExerciseName = "Overhead Press",
                    Template = exTempOHP,
                    Sets = new List<Set>
                    {
                        new Set { Weight = 95, Reps = 10, Template = exTempOHP.SetTemplates[0] },
                        new Set { Weight = 115, Reps = 8, Template = exTempOHP.SetTemplates[1] }
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

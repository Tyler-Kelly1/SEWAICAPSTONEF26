using Backend.Data;
using Backend.Models;
using Backend.Models.Templates;
using Microsoft.EntityFrameworkCore;

namespace Backend.Engines;

public class OverloadEngine : IOverloadEngine
{
    private readonly AppDbContext? _context;

    public OverloadEngine(AppDbContext context)
    {
        _context = context;
    }

    public OverloadEngine()
    {
        _context = null;
    }

    public async Task<Set_Value?> GetPreviousSetValueAsync(int? exerciseTemplateId, string? exerciseName, int setIndex, string? userId = null)
    {
        if (_context == null) return null;

        IQueryable<Exercise> query = _context.Exercises
            .Include(e => e.Workout)
            .Include(e => e.Sets);

        if (exerciseTemplateId.HasValue && exerciseTemplateId.Value > 0 && !string.IsNullOrWhiteSpace(exerciseName))
        {
            var nameLower = exerciseName.Trim().ToLower();
            query = query.Where(e => e.ExerciseTemplateId == exerciseTemplateId.Value ||
                                     (e.Template != null && e.Template.Id == exerciseTemplateId.Value) ||
                                     e.ExerciseName.ToLower() == nameLower ||
                                     (e.Template != null && e.Template.Exercise_Name.ToLower() == nameLower));
        }
        else if (exerciseTemplateId.HasValue && exerciseTemplateId.Value > 0)
        {
            query = query.Where(e => e.ExerciseTemplateId == exerciseTemplateId.Value ||
                                     (e.Template != null && e.Template.Id == exerciseTemplateId.Value));
        }
        else if (!string.IsNullOrWhiteSpace(exerciseName))
        {
            var nameLower = exerciseName.Trim().ToLower();
            query = query.Where(e => e.ExerciseName.ToLower() == nameLower ||
                                     (e.Template != null && e.Template.Exercise_Name.ToLower() == nameLower));
        }
        else
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(e => _context.Sessions.Any(s => s.ExecutedWorkoutId == e.WorkoutId && s.UserId == userId));
        }

        var mostRecentExercise = await query
            .OrderByDescending(e => e.Workout != null ? e.Workout.StartTime : DateTime.MinValue)
            .ThenByDescending(e => e.Id)
            .FirstOrDefaultAsync();

        if (mostRecentExercise == null || mostRecentExercise.Sets == null || mostRecentExercise.Sets.Count == 0)
        {
            return null;
        }

        var orderedSets = mostRecentExercise.Sets.OrderBy(s => s.Id).ToList();
        if (setIndex >= 0 && setIndex < orderedSets.Count)
        {
            var set = orderedSets[setIndex];
            return new Set_Value
            {
                Weight = set.Weight,
                Reps = set.Reps
            };
        }

        return null;
    }

    public Set_Value GetGoalSet(Exercise_Template exerciseTemp, Set_Template setTemp, Set_Value previousSet)
    {
        ArgumentNullException.ThrowIfNull(exerciseTemp);
        ArgumentNullException.ThrowIfNull(setTemp);
        ArgumentNullException.ThrowIfNull(previousSet);

        decimal weightStep = exerciseTemp.Weight_Step != 0 ? exerciseTemp.Weight_Step : 0.05m;
        int volumeStep = exerciseTemp.Volume_Step != 0 ? exerciseTemp.Volume_Step : 1;

        return GetGoalSet(
            previousSet.Reps,
            previousSet.Weight,
            setTemp.Min_Reps,
            setTemp.Max_Reps,
            weightStep,
            volumeStep
        );
    }

    public Set_Value GetGoalSet(int previousReps, int previousWeight, int minReps, int maxReps, decimal weightStep = 0.05m, int volumeStep = 1)
    {
        // Overload Ceiling
        if (previousReps >= maxReps)
        {
            int newWeight = (int)Math.Round(previousWeight * (1 + (double)weightStep));
            return new Set_Value { Weight = newWeight, Reps = minReps };
        }

        // Overload Volume
        if (previousReps >= minReps)
        {
            return new Set_Value { Weight = previousWeight, Reps = previousReps + volumeStep };
        }

        // Underload, dial the intensity back for recovery
        if (previousReps < minReps)
        {
            int newWeight = (int)Math.Round(previousWeight * (1 - (double)weightStep));
            return new Set_Value { Weight = newWeight, Reps = minReps };
        }

        return new Set_Value { Weight = previousWeight, Reps = previousReps };
    }
}

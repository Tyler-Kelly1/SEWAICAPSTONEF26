using Backend.Models;
using Backend.Models.Templates;

namespace Backend.Engines;

public interface IOverloadEngine
{
    Set_Value GetGoalSet(Exercise_Template exerciseTemp, Set_Template setTemp, Set_Value previousSet);
    Set_Value GetGoalSet(int previousReps, int previousWeight, int minReps, int maxReps, decimal weightStep = 0.05m, int volumeStep = 1);
    Task<Set_Value?> GetPreviousSetValueAsync(int? exerciseTemplateId, string? exerciseName, int setIndex, string? userId = null);
}

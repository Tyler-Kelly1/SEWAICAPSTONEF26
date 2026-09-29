

# Overload Method:

*Input*: Previous Reps, Previous Weight,Min Reps, Max Reps, Weight Step (default = 0.05), Volume Step (default = 1)

*Output*: Goal Reps and weight for the set (weight, reps)

```
// Overload Celling
if (previousReps >= maxReps) {
    return (previousWeight * (1+weightStep), minReps);
}

// Overload Volume
if (previousReps > minReps){
    return (previousWeight, previousReps + volumeStep);
}

// Underload, dial the intensitiy back for recovery
if (previousReps < minReps){
    return (previousWeight * (1-weightStep), minReps);
}
```



# Template Models

## Set_Template:

Failure_Set: bool

Max_Reps: int

Min_Reps: int



## Exercise_Template:

Exercise_Name: String

Max_Set: int

Min_Set: int

Weight_Step: decimal (0.01 < x < 0.99)

Volume_Step: int



# Data Models

## Set:

weight: int

reps: int

template: Set_Template

## Exercise:

Exercise_Name: String

Sets: List <set>

template: Exercise_Template

## 

## Workout:

Workout_Name: String

Exercise: List<Exercise>

StartTime: DateTime

EndTime: EndTime





## Session:

Executed_Workout: Workout

Session Date: DateTime

Duration => Executed_Workout.StartTime - Executed_Workout.EndTime

## User:

User_Id: string

Sessions: List <Session>

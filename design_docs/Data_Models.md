# Data Models



## Set:

weight: int

reps: int



## Exercise:

Exercise_Name: String

Sets: List<set>



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

Sessions: List<Session>

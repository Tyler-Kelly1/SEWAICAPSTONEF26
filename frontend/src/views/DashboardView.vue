<script setup>
    import { computed } from 'vue'

    const props = defineProps({
    sessions: {
    type: Array,
    default: () => []
    }
    })

    const totalWorkouts = computed(() => props.sessions.length)

    const totalExercises = computed(() => {
    return props.sessions.reduce((total, session) => {
    return total + (session.executedWorkout?.exercises?.length || 0)
    }, 0)
    })

    const totalSets = computed(() => {
    return props.sessions.reduce((sessionTotal, session) => {
    const exercises = session.executedWorkout?.exercises || []

    return sessionTotal + exercises.reduce((exerciseTotal, exercise) => {
    return exerciseTotal + (exercise.sets?.length || 0)
    }, 0)
    }, 0)
    })

    const totalWorkoutMinutes = computed(() => {
    return props.sessions.reduce((total, session) => {
    const duration = session.duration

    if (!duration || typeof duration !== 'string') {
    return total
    }

    const parts = duration.split(':')

    if (parts.length < 2) {
    return total
    }

    const hours = parseInt(parts[0], 10) || 0
    const minutes = parseInt(parts[1], 10) || 0

    return total + (hours * 60) + minutes
    }, 0)
    })

    const formattedWorkoutTime = computed(() => {
    const hours = Math.floor(totalWorkoutMinutes.value / 60)
    const minutes = totalWorkoutMinutes.value % 60

    if (hours > 0) {
    return `${hours}h ${minutes}m`
    }

    return `${minutes} min`
    })
</script>

<template>
    <v-card flat elevation="0" class="rounded-0 notebook-panel pa-4">
        <v-card-title class="text-h5 font-weight-bold mb-2">
            <v-icon icon="mdi-chart-box-outline" class="mr-2" color="primary"></v-icon>
            Workout Statistics
        </v-card-title>

        <v-card-subtitle class="mb-6">
            Summary of your workout history
        </v-card-subtitle>

        <v-row>
            <v-col cols="12" sm="6">
                <v-card class="stat-card rounded-0 pa-4" elevation="0">
                    <div class="text-caption">Total Workouts</div>
                    <div class="text-h4 font-weight-bold"
                         data-testid="total-workouts">
                        {{ totalWorkouts }}
                    </div>
                </v-card>
            </v-col>

            <v-col cols="12" sm="6">
                <v-card class="stat-card rounded-0 pa-4" elevation="0">
                    <div class="text-caption">Total Workout Time</div>
                    <div class="text-h4 font-weight-bold"
                         data-testid="total-workout-time">
                        {{ formattedWorkoutTime }}
                    </div>
                </v-card>
            </v-col>

            <v-col cols="12" sm="6">
                <v-card class="stat-card rounded-0 pa-4" elevation="0">
                    <div class="text-caption">Exercises Completed</div>
                    <div class="text-h4 font-weight-bold"
                         data-testid="total-exercises">
                        {{ totalExercises }}
                    </div>
                </v-card>
            </v-col>

            <v-col cols="12" sm="6">
                <v-card class="stat-card rounded-0 pa-4" elevation="0">
                    <div class="text-caption">Sets Completed</div>
                    <div class="text-h4 font-weight-bold"
                         data-testid="total-sets">
                        {{ totalSets }}
                    </div>
                </v-card>
            </v-col>
        </v-row>
    </v-card>
</template>

<style scoped>
    .notebook-panel {
        background-color: #f6ebd0 !important;
        border: 2px solid #1f1d18 !important;
        box-shadow: none !important;
    }

    .stat-card {
        background-color: #fbf2d3 !important;
        border: 1.5px solid #1f1d18 !important;
        box-shadow: none !important;
    }

    *,
    *::before,
    *::after {
        border-radius: 0 !important;
    }
</style>
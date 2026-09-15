<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  users: {
    type: Array,
    default: () => []
  },
  sessions: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['refresh'])

const selectedUserFilter = ref(null)

const filteredSessions = computed(() => {
  if (!selectedUserFilter.value) {
    return props.sessions
  }
  return props.sessions.filter(s => s.userId === selectedUserFilter.value)
})

const formatDate = (isoString) => {
  if (!isoString) return 'N/A'
  return new Date(isoString).toLocaleString()
}

const formatDuration = (durationStr) => {
  if (!durationStr) return 'N/A'
  // Expecting ISO duration string or TimeSpan representation "01:45:00"
  if (typeof durationStr === 'string' && durationStr.includes(':')) {
    const parts = durationStr.split(':')
    const hours = parseInt(parts[0], 10) || 0
    const minutes = parseInt(parts[1], 10) || 0
    if (hours > 0) return `${hours}h ${minutes}m`
    return `${minutes} min`
  }
  return durationStr
}
</script>

<template>
  <v-card class="rounded-lg elevation-4 pa-2">
    <v-card-item>
      <div class="d-flex align-center justify-space-between flex-wrap ga-2">
        <div>
          <v-card-title class="text-h6 font-weight-bold">
            <v-icon icon="mdi-dumbbell" class="mr-2" color="primary"></v-icon>
            Workout Sessions & Data Models
          </v-card-title>
          <v-card-subtitle>
            Viewing seeded data for User, Session, Workout, Exercise, and Set models
          </v-card-subtitle>
        </div>
        <div class="d-flex align-center ga-2">
          <v-btn-toggle
            v-model="selectedUserFilter"
            density="compact"
            color="primary"
            variant="outlined"
            mandatory="false"
          >
            <v-btn :value="null" size="small">All Users</v-btn>
            <v-btn
              v-for="user in users"
              :key="user.userId"
              :value="user.userId"
              size="small"
            >
              {{ user.userId }}
            </v-btn>
          </v-btn-toggle>
          <v-btn
            icon="mdi-refresh"
            variant="text"
            color="primary"
            :loading="loading"
            @click="emit('refresh')"
          ></v-btn>
        </div>
      </div>
    </v-card-item>

    <v-divider class="my-2"></v-divider>

    <v-card-text>
      <!-- Loading indicator -->
      <div v-if="loading" class="text-center py-8">
        <v-progress-circular indeterminate color="primary" size="48"></v-progress-circular>
        <div class="text-caption text-grey mt-2">Loading workout sessions...</div>
      </div>

      <!-- Empty state -->
      <div v-else-if="filteredSessions.length === 0" class="text-center py-8">
        <v-icon icon="mdi-emoticon-sad-outline" size="48" color="grey"></v-icon>
        <div class="text-subtitle-1 text-grey mt-2">No workout sessions found</div>
      </div>

      <!-- Workout Sessions List -->
      <v-expansion-panels v-else variant="inset" class="my-2">
        <v-expansion-panel
          v-for="session in filteredSessions"
          :key="session.id"
          class="mb-3 rounded-lg"
        >
          <v-expansion-panel-title>
            <div class="d-flex align-center justify-space-between w-100 pr-4">
              <div class="d-flex align-center ga-3">
                <v-avatar color="primary" variant="tonal" size="40">
                  <v-icon icon="mdi-account"></v-icon>
                </v-avatar>
                <div>
                  <div class="text-subtitle-1 font-weight-bold">
                    {{ session.executedWorkout?.workoutName || 'Unnamed Workout' }}
                  </div>
                  <div class="text-caption text-grey">
                    User: <strong class="text-primary">{{ session.userId }}</strong> &bull;
                    Date: {{ formatDate(session.sessionDate) }}
                  </div>
                </div>
              </div>

              <div class="d-flex align-center ga-2">
                <v-chip color="secondary" variant="outlined" size="small">
                  <v-icon icon="mdi-clock-outline" start size="x-small"></v-icon>
                  Duration: {{ formatDuration(session.duration) }}
                </v-chip>
                <v-chip color="info" variant="outlined" size="small">
                  Session #{{ session.id }}
                </v-chip>
              </div>
            </div>
          </v-expansion-panel-title>

          <v-expansion-panel-text>
            <div class="pa-2 bg-grey-darken-3 rounded-lg">
              <!-- Workout Details Header -->
              <div class="d-flex align-center justify-space-between mb-3 text-caption text-grey-lighten-1">
                <div>
                  <v-icon icon="mdi-play-circle-outline" size="x-small" class="mr-1"></v-icon>
                  Start: {{ formatDate(session.executedWorkout?.startTime) }}
                </div>
                <div>
                  <v-icon icon="mdi-stop-circle-outline" size="x-small" class="mr-1"></v-icon>
                  End: {{ formatDate(session.executedWorkout?.endTime) }}
                </div>
              </div>

              <!-- Exercises List -->
              <div
                v-for="exercise in session.executedWorkout?.exercises || []"
                :key="exercise.id"
                class="mb-3 pa-3 bg-grey-darken-4 rounded"
              >
                <div class="d-flex align-center justify-space-between mb-2">
                  <span class="text-subtitle-2 font-weight-bold text-light-blue-lighten-3">
                    <v-icon icon="mdi-weight-lifter" size="small" class="mr-1"></v-icon>
                    {{ exercise.exerciseName }}
                  </span>
                  <v-chip size="x-small" color="secondary">
                    {{ exercise.sets?.length || 0 }} Sets
                  </v-chip>
                </div>

                <!-- Sets Table -->
                <v-table density="compact" bg-color="transparent" class="rounded">
                  <thead>
                    <tr>
                      <th class="text-left text-caption">Set #</th>
                      <th class="text-left text-caption">Weight</th>
                      <th class="text-left text-caption">Reps</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(set, idx) in exercise.sets || []" :key="set.id || idx">
                      <td class="text-caption font-weight-bold">Set {{ idx + 1 }}</td>
                      <td class="text-caption">{{ set.weight }} lbs</td>
                      <td class="text-caption">{{ set.reps }} reps</td>
                    </tr>
                  </tbody>
                </v-table>
              </div>
            </div>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-card-text>
  </v-card>
</template>

<style scoped>
</style>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import SessionApi from '../services/Session.api.js'
import UserApi from '../services/User.api.js'
import ActiveWorkoutSession from '../components/ActiveWorkoutSession.vue'
import { useHashRouter } from '../router/index.js'

const props = defineProps({
  currentUserId: {
    type: String,
    default: ''
  },
  workoutTemplates: {
    type: Array,
    default: () => []
  },
  exerciseTemplates: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['save-session', 'session-saved'])

const router = useHashRouter()
const users = ref([])
const activeWorkoutTemplates = ref([])
const activeExerciseTemplates = ref([])
const loading = ref(false)
const submitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const selectedUser = ref(props.currentUserId || 'tyler_dev')
watch(() => props.currentUserId, (newUserId) => {
  if (newUserId) selectedUser.value = newUserId
})

const getBaselineWorkoutTemplates = (userId) => [
  {
    id: 1,
    userId: userId || 'tyler_dev',
    workout_Name: 'Upper Body Power Template',
    exerciseTemplates: [
      {
        id: 1,
        exercise_Name: 'Barbell Bench Press',
        min_Set: 3,
        max_Set: 5,
        weight_Step: 0.05,
        volume_Step: 1,
        setTemplates: [
          { id: 1, min_Reps: 6, max_Reps: 10, failure_Set: false },
          { id: 2, min_Reps: 6, max_Reps: 10, failure_Set: false },
          { id: 3, min_Reps: 4, max_Reps: 8, failure_Set: true }
        ]
      },
      {
        id: 2,
        exercise_Name: 'Bent Over Row',
        min_Set: 3,
        max_Set: 4,
        weight_Step: 0.05,
        volume_Step: 1,
        setTemplates: [
          { id: 4, min_Reps: 6, max_Reps: 10, failure_Set: false },
          { id: 5, min_Reps: 6, max_Reps: 10, failure_Set: false }
        ]
      }
    ]
  }
]

const getBaselineExerciseTemplates = () => [
  { id: 1, exercise_Name: 'Barbell Bench Press', min_Set: 3, max_Set: 5 },
  { id: 2, exercise_Name: 'Bent Over Row', min_Set: 3, max_Set: 4 }
]

const resolveActiveWorkoutTemplates = (fetchedTemplates) => {
  if (props.workoutTemplates && props.workoutTemplates.length > 0) {
    return props.workoutTemplates
  }
  if (fetchedTemplates && fetchedTemplates.length > 0) {
    return fetchedTemplates
  }
  return getBaselineWorkoutTemplates(selectedUser.value)
}

const resolveActiveExerciseTemplates = (fetchedTemplates) => {
  if (props.exerciseTemplates && props.exerciseTemplates.length > 0) {
    return props.exerciseTemplates
  }
  if (fetchedTemplates && fetchedTemplates.length > 0) {
    return fetchedTemplates
  }
  return getBaselineExerciseTemplates()
}

const fetchSessionDependencies = async (userId) => {
  return await Promise.all([
    UserApi.getUsers().catch(() => []),
    SessionApi.getWorkoutTemplates(userId).catch(() => []),
    SessionApi.getExerciseTemplates(userId).catch(() => [])
  ])
}

const loadSessionData = async () => {
  loading.value = true
  errorMessage.value = ''
  try {
    const [userData, fetchedWorkoutTemplates, fetchedExerciseTemplates] = await fetchSessionDependencies(selectedUser.value)
    users.value = userData || []
    activeWorkoutTemplates.value = resolveActiveWorkoutTemplates(fetchedWorkoutTemplates)
    activeExerciseTemplates.value = resolveActiveExerciseTemplates(fetchedExerciseTemplates)
  } catch (error) {
    console.error('Failed to load session dependencies:', error)
    errorMessage.value = error.message || 'Failed to initialize session view.'
  } finally {
    loading.value = false
  }
}

const handleFetchGoalSets = async ({ userId, exerciseName, exerciseTemplateId, callback }) => {
  try {
    const goalData = await SessionApi.getGoalSets({
      userId: userId || selectedUser.value,
      exerciseName,
      exerciseTemplateId,
      sets: []
    })
    if (callback) {
      callback(goalData)
    }
  } catch (err) {
    console.warn('Could not auto-fetch goal sets:', err)
  }
}

const handleSaveSession = async (sessionPayload) => {
  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const payload = {
      ...sessionPayload,
      userId: sessionPayload.userId || selectedUser.value
    }
    const saved = await SessionApi.createSession(payload)
    successMessage.value = `Workout Session #${saved.id} successfully recorded!`
    emit('save-session', payload)
    emit('session-saved', saved)
  } catch (err) {
    console.error('Error saving workout session:', err)
    errorMessage.value = err.message || 'Failed to record session with backend.'
  } finally {
    submitting.value = false
  }
}

const goToTemplates = () => {
  router.navigateTo('/templates')
}

onMounted(() => {
  loadSessionData()
})
</script>

<template>
  <div class="session-view-container" data-testid="session-view">
    <!-- View Header -->
    <v-card flat elevation="0" class="mb-4 rounded-0 notebook-panel pa-2">
      <v-card-item>
        <div class="d-flex align-center justify-space-between flex-wrap ga-2">
          <div>
            <v-card-title class="text-h6 font-weight-bold d-flex align-center">
              <v-icon icon="mdi-play-circle-outline" class="mr-2" color="primary"></v-icon>
              Active Session View
            </v-card-title>
            <v-card-subtitle>
              Interactive workout logging. <strong>Rule: Any active session MUST follow a template.</strong>
            </v-card-subtitle>
          </div>

          <div class="d-flex align-center ga-2">
            <v-btn
              variant="outlined"
              color="primary"
              size="small"
              class="rounded-0"
              prepend-icon="mdi-notebook-edit-outline"
              data-testid="btn-go-to-templates"
              @click="goToTemplates"
            >
              Manage Templates
            </v-btn>
          </div>
        </div>
      </v-card-item>
    </v-card>

    <!-- Status Alerts -->
    <v-alert
      v-if="errorMessage"
      type="error"
      variant="tonal"
      closable
      class="mb-4 rounded-0 unrounded-alert"
      @click:close="errorMessage = ''"
    >
      {{ errorMessage }}
    </v-alert>

    <v-alert
      v-if="successMessage"
      type="success"
      variant="tonal"
      closable
      class="mb-4 rounded-0 unrounded-alert"
      @click:close="successMessage = ''"
    >
      {{ successMessage }}
    </v-alert>

    <!-- Interactive Active Workout Session Component -->
    <ActiveWorkoutSession
      :users="users"
      :templates="activeExerciseTemplates"
      :workout-templates="activeWorkoutTemplates"
      :submitting="submitting"
      :current-user-id="selectedUser"
      @fetch-goal-sets="handleFetchGoalSets"
      @save-session="handleSaveSession"
    />
  </div>
</template>

<style scoped>
*, *::before, *::after {
  border-radius: 0 !important;
}

.session-view-container {
  max-width: 1000px;
  margin: 0 auto;
}

.unrounded-alert {
  border: 1.5px solid #1f1d18 !important;
  box-shadow: none !important;
}
</style>

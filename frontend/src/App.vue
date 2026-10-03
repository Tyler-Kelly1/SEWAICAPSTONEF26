<script setup>
import { ref, onMounted, watch } from 'vue'
import WorkoutSessionApi from './services/WorkoutSession.api.js'
import WorkoutSessionList from './components/WorkoutSessionList.vue'
import SessionView from './views/SessionView.vue'
import TemplateView from './views/TemplateView.vue'
import LoginView from './views/LoginView.vue'
import NotebookHeader from './components/NotebookHeader.vue'
import { useHashRouter } from './router/index.js'
import DashboardView from './views/DashboardView.vue'

const router = useHashRouter()
const activeTab = ref('start_session')

// Authentication / Active User State
const currentUserId = ref(
  (typeof localStorage !== 'undefined' && (localStorage.getItem('userId') || localStorage.getItem('overload_user_id'))) || ''
)

// Workout Sessions & Templates State
const users = ref([])
const sessions = ref([])
const templates = ref([
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
  },
  {
    id: 3,
    exercise_Name: 'Overhead Press',
    min_Set: 3,
    max_Set: 4,
    weight_Step: 0.05,
    volume_Step: 1,
    setTemplates: [
      { id: 6, min_Reps: 8, max_Reps: 12, failure_Set: false }
    ]
  }
])

const workoutTemplates = ref([
  {
    id: 1,
    userId: 'tyler_dev',
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
])

const fetchingWorkouts = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Check route guards on route or user change
watch([() => router.currentRoute.value, currentUserId], ([route, userId]) => {
  if (route === '/login' && userId) {
    router.navigateTo('/session')
  } else if (route !== '/login' && !userId) {
    router.navigateTo('/login')
  }
}, { immediate: true })

// Route synchronization with active tab
watch(() => router.currentRoute.value, (newRoute) => {
  if (newRoute === '/session' || newRoute === '/') {
    activeTab.value = 'start_session'
  } else if (newRoute === '/templates') {
    activeTab.value = 'workout_templates'
  } else if (newRoute === '/history') {
    activeTab.value = 'workouts'
    } else if (newRoute === '/dashboard') {
  activeTab.value = 'dashboard'
  }
}, { immediate: true })

watch(activeTab, (tab) => {
  if (router.currentRoute.value === '/login') return
  if (tab === 'start_session' && router.currentRoute.value !== '/session' && router.currentRoute.value !== '/') {
    router.navigateTo('/session')
  } else if (tab === 'workout_templates' && router.currentRoute.value !== '/templates') {
    router.navigateTo('/templates')
  } else if (tab === 'workouts' && router.currentRoute.value !== '/history') {
    router.navigateTo('/history')
} else if (tab === 'dashboard' && router.currentRoute.value !== '/dashboard') {
  router.navigateTo('/dashboard')
}
 
})

const fetchWorkoutData = async () => {
  fetchingWorkouts.value = true
  errorMessage.value = ''
  try {
    const [usersData, sessionsData, remoteTemplates, remoteWorkoutTemplates] = await Promise.all([
      WorkoutSessionApi.getUsers(),
      WorkoutSessionApi.getSessions(currentUserId.value || null),
      WorkoutSessionApi.getTemplates(currentUserId.value || null),
      WorkoutSessionApi.getWorkoutTemplates(currentUserId.value || null)
    ])
    users.value = usersData
    sessions.value = sessionsData

    if (remoteTemplates && remoteTemplates.length > 0) {
      templates.value = remoteTemplates
    }
    if (remoteWorkoutTemplates && remoteWorkoutTemplates.length > 0) {
      workoutTemplates.value = remoteWorkoutTemplates
    }
  } catch (error) {
    console.error('Error fetching Workout data:', error)
    errorMessage.value = error.message || 'Failed to load workout session data from API.'
  } finally {
    fetchingWorkouts.value = false
  }
}

const handleLoginSuccess = (userId) => {
  currentUserId.value = userId
  if (typeof localStorage !== 'undefined') {
    localStorage.setItem('userId', userId)
    localStorage.setItem('overload_user_id', userId)
  }
  successMessage.value = `Welcome, ${userId}!`
  router.navigateTo('/session')
  fetchWorkoutData()
}

const handleLogout = () => {
  if (typeof localStorage !== 'undefined') {
    localStorage.removeItem('userId')
    localStorage.removeItem('overload_user_id')
  }
  currentUserId.value = ''
  successMessage.value = 'Logged out. Please enter your User ID.'
  router.navigateTo('/login')
}

const handleTemplatesUpdated = (updatedTemplates) => {
  workoutTemplates.value = updatedTemplates
}

const handleSessionSaved = () => {
  fetchWorkoutData()
}

onMounted(() => {
  fetchWorkoutData()
})
</script>

<template>
  <v-app theme="scratchpad">
    <NotebookHeader
      title="SEW AI Capstone - Overload"
      subtitle="ANALOG GYM JOURNAL // VOL. 1"
      icon="mdi-dumbbell"
      :current-user-id="currentUserId"
      @switch-user="handleLogout"
    />

    <v-main class="bg-background drywall-bg">
      <v-container class="py-6" style="max-width: 1000px;">
        
        <!-- Status Alerts -->
        <v-alert
          v-if="errorMessage"
          type="error"
          variant="tonal"
          closable
          class="mb-4"
          @click:close="errorMessage = ''"
        >
          {{ errorMessage }}
        </v-alert>

        <v-alert
          v-if="successMessage"
          type="success"
          variant="tonal"
          closable
          class="mb-4"
          @click:close="successMessage = ''"
        >
          {{ successMessage }}
        </v-alert>

        <!-- Route: #/login -->
        <LoginView
          v-if="router.currentRoute.value === '/login'"
          :current-user-id="currentUserId"
          @login-success="handleLoginSuccess"
        />

        <!-- Route: Main Dashboard Views -->
        <template v-else>
          <!-- Navigation Tabs -->
          <v-tabs
            v-model="activeTab"
            color="primary"
            align-tabs="start"
            class="mb-6 border-b"
          >
            <v-tab value="start_session" data-testid="tab-start-session">
              <v-icon icon="mdi-play-circle-outline" start></v-icon>
              Session View
            </v-tab>
            <v-tab value="workout_templates" data-testid="tab-workout-templates">
              <v-icon icon="mdi-notebook-outline" start></v-icon>
              Template View
            </v-tab>
            <v-tab value="workouts" data-testid="tab-workouts">
              <v-icon icon="mdi-history" start></v-icon>
              Workout History
            </v-tab>
            <v-tab value="dashboard" data-testid="tab-dashboard">
                <v-icon icon="mdi-chart-box-outline" start></v-icon>
                Dashboard
            </v-tab>
          </v-tabs>

          <!-- Tab Windows -->
          <v-window v-model="activeTab">
              <!-- View 1: Session View (Active Session) -->
              <v-window-item value="start_session">
                  <SessionView :current-user-id="currentUserId"
                               :workout-templates="workoutTemplates"
                               :exercise-templates="templates"
                               @session-saved="handleSessionSaved" />
              </v-window-item>

              <!-- View 2: Template View (Edit, Create, Delete Templates) -->
              <v-window-item value="workout_templates">
                  <TemplateView :current-user-id="currentUserId"
                                @templates-updated="handleTemplatesUpdated" />
              </v-window-item>

              <!-- View 3: Workout History -->
              <v-window-item value="workouts">
                  <WorkoutSessionList :users="users"
                                      :sessions="sessions"
                                      :loading="fetchingWorkouts"
                                      :current-user-id="currentUserId"
                                      @refresh="fetchWorkoutData" />
              </v-window-item>

              <v-window-item value="dashboard">
                  <DashboardView :sessions="sessions" />
              </v-window-item>
          </v-window>
        </template>

      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
</style>

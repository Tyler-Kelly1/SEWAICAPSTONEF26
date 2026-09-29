<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  users: {
    type: Array,
    default: () => []
  },
  workoutTemplates: {
    type: Array,
    default: () => []
  },
  exerciseTemplates: {
    type: Array,
    default: () => []
  },
  submitting: {
    type: Boolean,
    default: false
  },
  currentUserId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['create-workout-template'])

const selectedUser = ref(props.currentUserId || (props.users.length > 0 ? props.users[0].userId : 'tyler_dev'))
watch(() => props.currentUserId, (newUserId) => {
  if (newUserId) selectedUser.value = newUserId
})
const activeUser = computed(() => props.currentUserId || selectedUser.value)
const workoutName = ref('')
const selectedExerciseIds = ref([])

// Available exercise templates
const userExerciseTemplates = computed(() => {
  return props.exerciseTemplates
})

// Filtered workout templates for selected user
const filteredWorkoutTemplates = computed(() => {
  return props.workoutTemplates.filter(workoutTemplate => !workoutTemplate.userId || workoutTemplate.userId === activeUser.value)
})

const handleSubmit = () => {
  if (!workoutName.value.trim() || selectedExerciseIds.value.length === 0) return

  const selectedExerciseObjs = props.exerciseTemplates.filter(exerciseTemplate => selectedExerciseIds.value.includes(exerciseTemplate.id || exerciseTemplate.exercise_Name))

  const payload = {
    userId: activeUser.value,
    workout_Name: workoutName.value.trim(),
    exerciseTemplates: selectedExerciseObjs.map(exerciseTemplate => ({
      id: exerciseTemplate.id,
      exercise_Name: exerciseTemplate.exercise_Name,
      min_Set: exerciseTemplate.min_Set || 3,
      max_Set: exerciseTemplate.max_Set || 5,
      weight_Step: exerciseTemplate.weight_Step || 0.05,
      volume_Step: exerciseTemplate.volume_Step || 1,
      setTemplates: exerciseTemplate.setTemplates || []
    }))
  }

  emit('create-workout-template', payload)

  workoutName.value = ''
  selectedExerciseIds.value = []
}
</script>

<template>
  <v-card flat elevation="0" class="rounded-0 notebook-panel pa-2">
    <v-card-item>
      <div class="d-flex align-center justify-space-between flex-wrap ga-2">
        <div>
          <v-card-title class="text-h6 font-weight-bold">
            <v-icon icon="mdi-notebook-outline" class="mr-2" color="primary"></v-icon>
            Workout Templates Management
          </v-card-title>
          <v-card-subtitle>
            Create and view full Workout Templates composed of multiple Exercise Templates
          </v-card-subtitle>
        </div>

        <v-chip
          v-if="currentUserId"
          color="primary"
          variant="tonal"
          size="small"
          class="rounded-0 font-weight-bold"
          data-testid="workout-manager-user-chip"
        >
          <v-icon icon="mdi-account" start size="x-small"></v-icon>
          Athlete: {{ currentUserId }}
        </v-chip>
        <v-select
          v-else-if="users.length > 0"
          v-model="selectedUser"
          :items="users.map(u => u.userId)"
          label="Selected User"
          variant="outlined"
          density="compact"
          hide-details
          style="min-width: 180px;"
        ></v-select>
      </div>
    </v-card-item>

    <v-divider class="my-2"></v-divider>

    <v-card-text>
      <!-- Form to Create Workout Template -->
      <v-form class="mb-6 notebook-subpanel pa-4 rounded-0" @submit.prevent="handleSubmit">
        <div class="text-subtitle-1 font-weight-bold text-primary mb-3">
          <v-icon icon="mdi-plus-circle" class="mr-1"></v-icon>
          Create New Workout Template
        </div>

        <v-row align="center">
          <v-col cols="12" md="6">
            <v-text-field
              v-model="workoutName"
              data-testid="workout-template-name-input"
              label="Workout Template Name"
              placeholder="e.g. Upper Body Hypertrophy"
              variant="outlined"
              density="compact"
              required
            ></v-text-field>
          </v-col>

          <v-col cols="12" md="6">
            <div class="text-caption text-grey mb-1">
              Select Exercise Templates to include:
            </div>
            <v-select
              v-model="selectedExerciseIds"
              data-testid="workout-template-exercises-select"
              :items="userExerciseTemplates"
              item-title="exercise_Name"
              item-value="id"
              label="Included Exercise Templates"
              variant="outlined"
              density="compact"
              multiple
              chips
              closable-chips
            ></v-select>
          </v-col>
        </v-row>

        <div class="text-right mt-2">
          <v-btn
            type="submit"
            data-testid="create-workout-template-btn"
            color="primary"
            class="rounded-0"
            prepend-icon="mdi-content-save"
            :loading="submitting"
            :disabled="!workoutName.trim() || selectedExerciseIds.length === 0"
          >
            Create Workout Template
          </v-btn>
        </div>
      </v-form>

      <!-- List of Existing Workout Templates -->
      <div class="text-subtitle-1 font-weight-bold mb-3">
        Existing Workout Templates ({{ filteredWorkoutTemplates.length }})
      </div>

      <div v-if="filteredWorkoutTemplates.length === 0" class="text-center py-6 text-grey">
        <v-icon icon="mdi-text-box-remove-outline" size="40" class="d-block mx-auto mb-2"></v-icon>
        No workout templates created for {{ selectedUser }} yet. Create one above!
      </div>

      <v-row v-else>
        <v-col
          v-for="wt in filteredWorkoutTemplates"
          :key="wt.id || wt.workout_Name"
          cols="12"
          md="6"
        >
          <v-card flat elevation="0" class="rounded-0 notebook-subpanel pa-3 fill-height">
            <div class="d-flex align-center justify-space-between mb-2">
              <div class="text-subtitle-1 font-weight-bold text-primary">
                <v-icon icon="mdi-format-list-bulleted-type" class="mr-1"></v-icon>
                {{ wt.workout_Name }}
              </div>
              <v-chip size="x-small" color="secondary" class="rounded-0">
                {{ wt.exerciseTemplates ? wt.exerciseTemplates.length : 0 }} Exercises
              </v-chip>
            </div>

            <v-divider class="my-2"></v-divider>

            <div class="text-caption font-weight-bold text-on-surface-variant mb-1">
              Included Exercise Templates:
            </div>
            <v-list density="compact" bg-color="transparent" class="py-0">
              <v-list-item
                v-for="et in wt.exerciseTemplates || []"
                :key="et.id || et.exercise_Name"
                class="px-0 py-1"
              >
                <template v-slot:prepend>
                  <v-icon icon="mdi-dumbbell" size="x-small" color="primary" class="mr-2"></v-icon>
                </template>
                <v-list-item-title class="text-caption font-weight-bold">
                  {{ et.exercise_Name }}
                </v-list-item-title>
                <v-list-item-subtitle class="text-caption text-grey">
                  {{ et.min_Set }}-{{ et.max_Set }} sets &bull; {{ et.setTemplates ? et.setTemplates.length : 0 }} set prescriptions
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<style scoped>
*, *::before, *::after {
  border-radius: 0 !important;
}

.notebook-panel {
  background-color: #f6ebd0 !important;
  border: 2px solid #1f1d18 !important;
  border-radius: 0 !important;
  box-shadow: none !important;
}

.notebook-subpanel {
  background-color: #fbf2d3 !important;
  border: 1.5px solid #1f1d18 !important;
  border-radius: 0 !important;
  box-shadow: none !important;
}
</style>

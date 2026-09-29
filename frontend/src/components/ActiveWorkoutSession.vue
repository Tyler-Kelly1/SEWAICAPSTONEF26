<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  users: {
    type: Array,
    default: () => []
  },
  templates: {
    type: Array,
    default: () => []
  },
  workoutTemplates: {
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

const emit = defineEmits(['fetch-goal-sets', 'save-session'])

const selectedUser = ref(props.currentUserId || (props.users.length > 0 ? props.users[0].userId : 'tyler_dev'))
watch(() => props.currentUserId, (newUserId) => {
  if (newUserId) selectedUser.value = newUserId
})
const selectedWorkoutTemplateId = ref(null)
const workoutName = ref('Push Day Session')
const sessionStarted = ref(false)
const startTime = ref(null)

// Workout templates for active session
const userWorkoutTemplates = computed(() => {
  return props.workoutTemplates
})

watch(() => props.workoutTemplates, (newTemplates) => {
  if (newTemplates && newTemplates.length > 0 && !selectedWorkoutTemplateId.value) {
    selectedWorkoutTemplateId.value = newTemplates[0].id || newTemplates[0].workout_Name
    applyWorkoutTemplate(selectedWorkoutTemplateId.value)
  }
}, { immediate: true })

// Exercise structure for active session
const exercises = ref([
  {
    name: 'Barbell Bench Press',
    templateId: null,
    sets: [
      { setNumber: 1, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false },
      { setNumber: 2, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false },
      { setNumber: 3, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false }
    ]
  },
  {
    name: 'Bent Over Row',
    templateId: null,
    sets: [
      { setNumber: 1, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false },
      { setNumber: 2, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false },
      { setNumber: 3, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false }
    ]
  }
])

const applyWorkoutTemplate = (templateId) => {
  if (!templateId) return
  const matchedWorkoutTemplate = props.workoutTemplates.find(template => (template.id || template.workout_Name) === templateId)
  if (!matchedWorkoutTemplate) return

  workoutName.value = matchedWorkoutTemplate.workout_Name
  if (matchedWorkoutTemplate.exerciseTemplates && matchedWorkoutTemplate.exerciseTemplates.length > 0) {
    exercises.value = matchedWorkoutTemplate.exerciseTemplates.map(exerciseTemplate => {
      const setCount = exerciseTemplate.setTemplates?.length || exerciseTemplate.min_Set || 3
      const setList = []
      for (let setIndex = 0; setIndex < setCount; setIndex++) {
        setList.push({
          setNumber: setIndex + 1,
          weight: 0,
          reps: 0,
          completed: false,
          goalWeight: null,
          goalReps: null,
          loadingGoal: false
        })
      }
      return {
        name: exerciseTemplate.exercise_Name,
        templateId: exerciseTemplate.id,
        sets: setList
      }
    })
  }
}

const addExercise = () => {
  exercises.value.push({
    name: 'Custom Exercise',
    templateId: null,
    sets: [
      { setNumber: 1, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false },
      { setNumber: 2, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false },
      { setNumber: 3, weight: 0, reps: 0, completed: false, goalWeight: null, goalReps: null, loadingGoal: false }
    ]
  })
}

const addSetToExercise = (exIndex) => {
  const setsList = exercises.value[exIndex].sets
  const nextSetNum = setsList.length + 1
  setsList.push({
    setNumber: nextSetNum,
    weight: 0,
    reps: 0,
    completed: false,
    goalWeight: null,
    goalReps: null,
    loadingGoal: false
  })
}

const isSetEnabled = (exIndex, setIndex) => {
  if (!sessionStarted.value) return false
  if (setIndex === 0) return true
  return exercises.value[exIndex].sets[setIndex - 1].completed
}

const startSession = () => {
  sessionStarted.value = true
  startTime.value = new Date()

  exercises.value.forEach((exercise, exerciseIndex) => {
    fetchGoalWeightForExercise(exerciseIndex)
  })
}

const fetchGoalWeightForExercise = async (exerciseIndex) => {
  const targetExercise = exercises.value[exerciseIndex]
  if (!targetExercise || !targetExercise.name) return

  const firstSet = targetExercise.sets[0]
  if (firstSet) {
    firstSet.loadingGoal = true
  }

  emit('fetch-goal-sets', {
    userId: selectedUser.value,
    exerciseName: targetExercise.name,
    exerciseTemplateId: targetExercise.templateId,
    callback: (goalResponse) => {
      if (firstSet) {
        firstSet.loadingGoal = false
      }
      if (goalResponse && goalResponse.goalSets && goalResponse.goalSets.length > 0) {
        goalResponse.goalSets.forEach((goalSet, setIndex) => {
          if (targetExercise.sets[setIndex]) {
            targetExercise.sets[setIndex].goalWeight = goalSet.weight
            targetExercise.sets[setIndex].goalReps = goalSet.reps
            if (!targetExercise.sets[setIndex].completed && targetExercise.sets[setIndex].weight === 0) {
              targetExercise.sets[setIndex].weight = goalSet.weight
              targetExercise.sets[setIndex].reps = goalSet.reps
            }
          }
        })
      }
    }
  })
}

const completeSet = (exerciseIndex, setIndex) => {
  const set = exercises.value[exerciseIndex].sets[setIndex]
  set.completed = !set.completed
}

const finishWorkout = () => {
  const endTime = new Date()
  const payload = {
    userId: selectedUser.value,
    workoutName: workoutName.value || 'Workout Session',
    startTime: startTime.value ? startTime.value.toISOString() : new Date().toISOString(),
    endTime: endTime.toISOString(),
    exercises: exercises.value.map(exercise => ({
      exerciseName: exercise.name,
      exerciseTemplateId: exercise.templateId,
      sets: exercise.sets
        .filter(setRecord => setRecord.completed || setRecord.weight > 0)
        .map(setRecord => ({
          weight: parseInt(setRecord.weight, 10) || 0,
          reps: parseInt(setRecord.reps, 10) || 0
        }))
    })).filter(exercise => exercise.sets.length > 0)
  }

  emit('save-session', payload)
  sessionStarted.value = false
  selectedWorkoutTemplateId.value = null
  startTime.value = null
}
</script>

<template>
  <v-card flat elevation="0" class="rounded-0 notebook-panel pa-2">
    <v-card-item>
      <div class="d-flex align-center justify-space-between flex-wrap ga-2">
        <div>
          <v-card-title class="text-h6 font-weight-bold">
            <v-icon icon="mdi-play-circle" class="mr-2" color="success"></v-icon>
            Start Exercise Session
          </v-card-title>
          <v-card-subtitle>
            Perform sets in sequential order. Active first sets automatically fetch goal weight from Backend.
          </v-card-subtitle>
        </div>

        <div v-if="sessionStarted" class="d-flex align-center ga-2">
          <v-chip color="success" variant="flat" size="small" class="rounded-0">
            <v-icon icon="mdi-record-circle" start size="x-small"></v-icon>
            Session In Progress
          </v-chip>
          <v-btn
            color="primary"
            prepend-icon="mdi-check-all"
            class="rounded-0"
            :loading="submitting"
            @click="finishWorkout"
          >
            Finish & Save Workout
          </v-btn>
        </div>
      </div>
    </v-card-item>

    <v-divider class="my-2"></v-divider>

    <v-card-text>
      <!-- Pre-Session Configuration -->
      <v-form v-if="!sessionStarted" @submit.prevent="startSession">
        <v-row align="center">
          <v-col cols="12" md="4">
            <v-select
              v-model="selectedUser"
              :items="users.map(u => u.userId)"
              label="Select User"
              variant="outlined"
              density="compact"
            ></v-select>
          </v-col>

          <v-col cols="12" md="4">
            <v-select
              v-model="selectedWorkoutTemplateId"
              data-testid="select-workout-template"
              :items="userWorkoutTemplates"
              item-title="workout_Name"
              item-value="id"
              label="Select Workout Template (Mandatory)"
              variant="outlined"
              density="compact"
              clearable
              :rules="[v => !!v || 'Any active session MUST follow a template.']"
              @update:model-value="applyWorkoutTemplate"
            ></v-select>
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model="workoutName"
              data-testid="workout-session-name-input"
              label="Workout Session Name"
              variant="outlined"
              density="compact"
            ></v-text-field>
          </v-col>
        </v-row>

        <div class="d-flex align-center justify-space-between mb-3">
          <div class="text-subtitle-2 font-weight-bold">Planned Exercises</div>
          <v-btn size="small" variant="tonal" color="primary" class="rounded-0" prepend-icon="mdi-plus" @click="addExercise">
            Add Exercise
          </v-btn>
        </div>

        <div
          v-for="(ex, exIdx) in exercises"
          :key="exIdx"
          class="mb-3 pa-3 notebook-subpanel rounded-0"
        >
          <v-row align="center" density="compact">
            <v-col cols="12" md="6">
              <v-text-field
                v-model="ex.name"
                label="Exercise Name"
                variant="outlined"
                density="compact"
                hide-details
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6" class="text-right">
              <v-chip size="small" color="info" class="mr-2 rounded-0">
                {{ ex.sets.length }} Required Sets
              </v-chip>
              <v-btn size="small" color="secondary" variant="text" class="rounded-0" prepend-icon="mdi-plus" @click="addSetToExercise(exIdx)">
                Add Set
              </v-btn>
            </v-col>
          </v-row>
        </div>

        <v-btn
          color="success"
          size="large"
          block
          class="mt-4 rounded-0"
          prepend-icon="mdi-play"
          type="submit"
          data-testid="begin-workout-session-btn"
          :disabled="!selectedWorkoutTemplateId && userWorkoutTemplates.length > 0"
        >
          Begin Workout Session
        </v-btn>
      </v-form>

      <!-- Active Workout Execution Phase -->
      <div v-else>
        <div class="d-flex align-center justify-space-between mb-4">
          <div class="text-subtitle-1 font-weight-bold text-primary">
            User: <strong>{{ selectedUser }}</strong> &bull; {{ workoutName }}
          </div>
        </div>

        <div
          v-for="(ex, exIdx) in exercises"
          :key="exIdx"
          class="mb-4 pa-4 notebook-subpanel rounded-0"
        >
          <div class="d-flex align-center justify-space-between mb-3">
            <div class="text-h6 font-weight-bold text-primary">
              <v-icon icon="mdi-weight-lifter" class="mr-2"></v-icon>
              {{ ex.name }}
            </div>
            <v-btn
              size="x-small"
              color="info"
              variant="outlined"
              class="rounded-0"
              prepend-icon="mdi-target"
              :loading="ex.sets[0]?.loadingGoal"
              @click="fetchGoalWeightForExercise(exIdx)"
            >
              Fetch BE Goal Weights
            </v-btn>
          </div>

          <v-table density="compact" bg-color="transparent" class="rounded-0 notebook-table">
            <thead>
              <tr>
                <th class="text-left">Set #</th>
                <th class="text-left">BE Goal Weight</th>
                <th class="text-left">BE Goal Reps</th>
                <th class="text-left">Input Weight (lbs)</th>
                <th class="text-left">Input Reps</th>
                <th class="text-left">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(set, setIdx) in ex.sets"
                :key="setIdx"
                data-testid="set-row"
                :class="{ 'disabled-set-row': !isSetEnabled(exIdx, setIdx) }"
              >
                <td class="font-weight-bold">
                  Set {{ set.setNumber }}
                  <v-chip v-if="setIdx === 0" size="x-small" color="primary" class="ml-1 rounded-0">
                    First / Active
                  </v-chip>
                </td>
                <td>
                  <span v-if="set.goalWeight !== null" data-testid="goal-weight" class="text-success font-weight-bold">
                    {{ set.goalWeight }} lbs
                  </span>
                  <span v-else class="text-caption text-grey">Auto-fetching...</span>
                </td>
                <td>
                  <span v-if="set.goalReps !== null" data-testid="goal-reps" class="text-success font-weight-bold">
                    {{ set.goalReps }} reps
                  </span>
                  <span v-else class="text-caption text-grey">Auto-fetching...</span>
                </td>
                <td>
                  <v-text-field
                    v-model.number="set.weight"
                    data-testid="set-weight-input"
                    type="number"
                    density="compact"
                    variant="outlined"
                    hide-details
                    style="max-width: 110px;"
                    :disabled="!isSetEnabled(exIdx, setIdx) || set.completed"
                  ></v-text-field>
                </td>
                <td>
                  <v-text-field
                    v-model.number="set.reps"
                    data-testid="set-reps-input"
                    type="number"
                    density="compact"
                    variant="outlined"
                    hide-details
                    style="max-width: 100px;"
                    :disabled="!isSetEnabled(exIdx, setIdx) || set.completed"
                  ></v-text-field>
                </td>
                <td>
                  <v-btn
                    v-if="!set.completed"
                    size="small"
                    color="success"
                    variant="tonal"
                    class="rounded-0"
                    data-testid="complete-set-btn"
                    prepend-icon="mdi-check"
                    :disabled="!isSetEnabled(exIdx, setIdx)"
                    @click="completeSet(exIdx, setIdx)"
                  >
                    Complete Set
                  </v-btn>
                  <v-chip v-else color="success" size="small" class="rounded-0" closable @click:close="completeSet(exIdx, setIdx)">
                    Done
                  </v-chip>
                </td>
              </tr>
            </tbody>
          </v-table>
        </div>

        <div class="text-right mt-4">
          <v-btn
            color="primary"
            size="large"
            class="rounded-0"
            prepend-icon="mdi-check-all"
            data-testid="finish-workout-btn"
            :loading="submitting"
            @click="finishWorkout"
          >
            Finish & Save Workout
          </v-btn>
        </div>
      </div>
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

.notebook-table {
  border: 1px solid #1f1d18 !important;
  border-radius: 0 !important;
  background-color: #fcf5dc !important;
}

.notebook-table th {
  background-color: #f0e1b9 !important;
  color: #1f1d18 !important;
  border-bottom: 1.5px solid #1f1d18 !important;
}

.notebook-table td {
  border-bottom: 1px solid #cdbe8d !important;
  color: #1f1d18 !important;
}

.disabled-set-row {
  background-color: #ebdca6 !important;
  opacity: 0.65;
}
</style>

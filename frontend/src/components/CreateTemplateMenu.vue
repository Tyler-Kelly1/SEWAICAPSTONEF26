<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  users: {
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

const emit = defineEmits(['create-template'])

const userId = ref(props.currentUserId || (props.users.length > 0 ? props.users[0].userId : 'tyler_dev'))
watch(() => props.currentUserId, (newUserId) => {
  if (newUserId) userId.value = newUserId
})
const effectiveUserId = computed(() => props.currentUserId || userId.value)
const exerciseName = ref('')
const minSet = ref(3)
const maxSet = ref(5)
const weightStepPct = ref(5) // default 5% -> 0.05
const volumeStep = ref(1)

const setTemplates = ref([
  { min_Reps: 6, max_Reps: 10, failure_Set: false },
  { min_Reps: 6, max_Reps: 10, failure_Set: false },
  { min_Reps: 4, max_Reps: 8, failure_Set: true }
])

const addSetTemplate = () => {
  setTemplates.value.push({
    min_Reps: 6,
    max_Reps: 10,
    failure_Set: false
  })
}

const removeSetTemplate = (index) => {
  if (setTemplates.value.length > 1) {
    setTemplates.value.splice(index, 1)
  }
}

const handleSubmit = () => {
  if (!exerciseName.value.trim()) return

  const payload = {
    userId: effectiveUserId.value,
    exercise_Name: exerciseName.value.trim(),
    min_Set: parseInt(minSet.value, 10) || 1,
    max_Set: parseInt(maxSet.value, 10) || 5,
    weight_Step: parseFloat(weightStepPct.value) / 100 || 0.05,
    volume_Step: parseInt(volumeStep.value, 10) || 1,
    setTemplates: setTemplates.value.map(setTemplate => ({
      min_Reps: parseInt(setTemplate.min_Reps, 10) || 1,
      max_Reps: parseInt(setTemplate.max_Reps, 10) || 10,
      failure_Set: !!setTemplate.failure_Set
    }))
  }

  emit('create-template', payload)

  // Reset basic form fields
  exerciseName.value = ''
}
</script>

<template>
  <v-card flat elevation="0" class="rounded-0 notebook-panel pa-2">
    <v-card-item>
      <v-card-title class="text-h6 font-weight-bold">
        <v-icon icon="mdi-plus-box-multiple-outline" class="mr-2" color="primary"></v-icon>
        Create Exercise Template Menu
      </v-card-title>
      <v-card-subtitle>
        Define progressive overload rules and set targets for the currently logged-in user
      </v-card-subtitle>
    </v-card-item>

    <v-divider class="my-2"></v-divider>

    <v-card-text>
      <v-form @submit.prevent="handleSubmit">
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              :model-value="effectiveUserId"
              label="Athlete / User"
              variant="outlined"
              density="compact"
              readonly
              disabled
              prepend-inner-icon="mdi-account"
              hint="Template is created for the currently logged-in user"
              persistent-hint
              data-testid="target-user-field"
            ></v-text-field>
          </v-col>

          <v-col cols="12" md="6">
            <v-text-field
              v-model="exerciseName"
              label="Exercise Name"
              placeholder="e.g. Incline Dumbbell Press"
              variant="outlined"
              density="compact"
              prepend-inner-icon="mdi-dumbbell"
              required
            ></v-text-field>
          </v-col>

          <v-col cols="6" md="3">
            <v-text-field
              v-model.number="minSet"
              type="number"
              label="Min Sets"
              variant="outlined"
              density="compact"
              min="1"
              max="20"
            ></v-text-field>
          </v-col>

          <v-col cols="6" md="3">
            <v-text-field
              v-model.number="maxSet"
              type="number"
              label="Max Sets"
              variant="outlined"
              density="compact"
              min="1"
              max="20"
            ></v-text-field>
          </v-col>

          <v-col cols="6" md="3">
            <v-text-field
              v-model.number="weightStepPct"
              type="number"
              label="Weight Step (%)"
              variant="outlined"
              density="compact"
              suffix="%"
              step="0.5"
              min="0.1"
              max="50"
            ></v-text-field>
          </v-col>

          <v-col cols="6" md="3">
            <v-text-field
              v-model.number="volumeStep"
              type="number"
              label="Volume Step (Reps)"
              variant="outlined"
              density="compact"
              min="1"
              max="10"
            ></v-text-field>
          </v-col>
        </v-row>

        <v-divider class="my-4"></v-divider>

        <div class="d-flex align-center justify-space-between mb-3">
          <div class="text-subtitle-1 font-weight-bold text-primary">
            Set Prescriptions ({{ setTemplates.length }})
          </div>
          <v-btn
            size="small"
            color="secondary"
            variant="tonal"
            class="rounded-0"
            prepend-icon="mdi-plus"
            @click="addSetTemplate"
          >
            Add Set Template
          </v-btn>
        </div>

        <v-row v-for="(setTemp, index) in setTemplates" :key="index" align="center" class="mb-2 notebook-subpanel rounded-0 pa-2">
          <v-col cols="12" sm="1" class="font-weight-bold text-caption">
            Set {{ index + 1 }}
          </v-col>
          <v-col cols="6" sm="3">
            <v-text-field
              v-model.number="setTemp.min_Reps"
              type="number"
              label="Min Reps"
              variant="outlined"
              density="compact"
              hide-details
              min="1"
            ></v-text-field>
          </v-col>
          <v-col cols="6" sm="3">
            <v-text-field
              v-model.number="setTemp.max_Reps"
              type="number"
              label="Max Reps"
              variant="outlined"
              density="compact"
              hide-details
              min="1"
            ></v-text-field>
          </v-col>
          <v-col cols="8" sm="3">
            <v-checkbox
              v-model="setTemp.failure_Set"
              label="Failure Set?"
              density="compact"
              hide-details
              color="error"
            ></v-checkbox>
          </v-col>
          <v-col cols="4" sm="2" class="text-right">
            <v-btn
              icon="mdi-delete-outline"
              size="small"
              color="error"
              variant="text"
              class="rounded-0"
              :disabled="setTemplates.length <= 1"
              @click="removeSetTemplate(index)"
            ></v-btn>
          </v-col>
        </v-row>

        <v-row class="mt-4">
          <v-col cols="12" class="text-right">
            <v-btn
              type="submit"
              color="primary"
              size="large"
              class="rounded-0"
              prepend-icon="mdi-content-save"
              :loading="submitting"
              :disabled="!exerciseName.trim()"
            >
              Save Exercise Template
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
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

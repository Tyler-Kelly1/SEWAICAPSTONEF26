<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import TemplateApi from '../services/Template.api.js'
import UserApi from '../services/User.api.js'
import CreateTemplateMenu from '../components/CreateTemplateMenu.vue'
import ExerciseTemplateList from '../components/ExerciseTemplateList.vue'

const props = defineProps({
  currentUserId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['templates-updated'])

const users = ref([])
const workoutTemplates = ref([])
const exerciseTemplates = ref([])
const loading = ref(false)
const submitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const activeUser = computed(() => props.currentUserId || 'tyler_dev')
const activeTab = ref('workout_templates')

// Create Workout Template form
const newWorkoutName = ref('')
const selectedExerciseIds = ref([])

// Edit Workout Template dialog state
const editDialogOpen = ref(false)
const editingTemplateId = ref(null)
const editWorkoutName = ref('')
const editSelectedExerciseIds = ref([])

// Delete Confirmation dialog state
const deleteDialogOpen = ref(false)
const deletingTemplate = ref(null)

const userWorkoutTemplates = computed(() => {
  const targetUser = activeUser.value
  return workoutTemplates.value.filter(workoutTemplate => !workoutTemplate.userId || workoutTemplate.userId === targetUser)
})

const loadData = async () => {
  loading.value = true
  errorMessage.value = ''
  const targetUser = activeUser.value
  try {
    const [userData, wtData, etData] = await Promise.all([
      UserApi.getUsers().catch(() => []),
      TemplateApi.getWorkoutTemplates(targetUser).catch(() => []),
      TemplateApi.getExerciseTemplates(targetUser).catch(() => [])
    ])
    users.value = userData || []
    
    // Provide baseline sample templates if backend returns empty list
    if (wtData && wtData.length > 0) {
      workoutTemplates.value = wtData
    } else if (workoutTemplates.value.length === 0) {
      workoutTemplates.value = [
        {
          id: 1,
          userId: targetUser,
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
    }

    if (etData && etData.length > 0) {
      exerciseTemplates.value = etData
    } else if (exerciseTemplates.value.length === 0) {
      exerciseTemplates.value = [
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
      ]
    }
  } catch (err) {
    console.error('Failed to load template data:', err)
    errorMessage.value = err.message || 'Failed to load templates from server.'
  } finally {
    loading.value = false
  }
}

// 1. CREATE Workout Template
const handleCreateWorkoutTemplate = async () => {
  if (!newWorkoutName.value.trim() || selectedExerciseIds.value.length === 0) return

  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const selectedExerciseObjs = exerciseTemplates.value.filter(exerciseTemplate => 
      selectedExerciseIds.value.includes(exerciseTemplate.id || exerciseTemplate.exercise_Name)
    )

    const payload = {
      userId: activeUser.value,
      workout_Name: newWorkoutName.value.trim(),
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

    const created = await TemplateApi.createWorkoutTemplate(payload)
    workoutTemplates.value.push(created)
    successMessage.value = `Successfully created Workout Template "${payload.workout_Name}"!`
    newWorkoutName.value = ''
    selectedExerciseIds.value = []
    emit('templates-updated', workoutTemplates.value)
  } catch (err) {
    console.error('Error creating workout template:', err)
    errorMessage.value = err.message || 'Failed to create workout template.'
  } finally {
    submitting.value = false
  }
}

// 2. EDIT Workout Template
const openEditDialog = (workoutTemplate) => {
  editingTemplateId.value = workoutTemplate.id
  editWorkoutName.value = workoutTemplate.workout_Name
  editSelectedExerciseIds.value = (workoutTemplate.exerciseTemplates || []).map(exerciseTemplate => exerciseTemplate.id || exerciseTemplate.exercise_Name)
  editDialogOpen.value = true
}

const buildUpdatedExerciseList = (selectedIds, availableTemplates) => {
  const matchedExercises = availableTemplates.filter(exerciseTemplate =>
    selectedIds.includes(exerciseTemplate.id || exerciseTemplate.exercise_Name)
  )

  return matchedExercises.map(exerciseTemplate => ({
    id: exerciseTemplate.id,
    exercise_Name: exerciseTemplate.exercise_Name,
    min_Set: exerciseTemplate.min_Set || 3,
    max_Set: exerciseTemplate.max_Set || 5,
    weight_Step: exerciseTemplate.weight_Step || 0.05,
    volume_Step: exerciseTemplate.volume_Step || 1,
    setTemplates: exerciseTemplate.setTemplates || []
  }))
}

const buildWorkoutTemplateEditPayload = (templateId, userId, workoutName, updatedExercises) => ({
  id: templateId,
  userId,
  workout_Name: workoutName.trim(),
  exerciseTemplates: updatedExercises
})

const updateLocalWorkoutTemplate = (updatedPayload) => {
  const index = workoutTemplates.value.findIndex(workoutTemplate => workoutTemplate.id === updatedPayload.id)
  if (index !== -1) {
    workoutTemplates.value[index] = { ...workoutTemplates.value[index], ...updatedPayload }
  }
}

const handleSaveEdit = async () => {
  if (!editWorkoutName.value.trim() || editSelectedExerciseIds.value.length === 0) return

  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const updatedExercises = buildUpdatedExerciseList(editSelectedExerciseIds.value, exerciseTemplates.value)
    const payload = buildWorkoutTemplateEditPayload(editingTemplateId.value, activeUser.value, editWorkoutName.value, updatedExercises)

    await TemplateApi.updateWorkoutTemplate(editingTemplateId.value, payload)
    updateLocalWorkoutTemplate(payload)

    successMessage.value = `Successfully updated Workout Template "${payload.workout_Name}"!`
    editDialogOpen.value = false
    emit('templates-updated', workoutTemplates.value)
  } catch (error) {
    console.error('Error updating workout template:', error)
    errorMessage.value = error.message || 'Failed to update workout template.'
  } finally {
    submitting.value = false
  }
}

// 3. DELETE Workout Template
const confirmDelete = (workoutTemplate) => {
  deletingTemplate.value = workoutTemplate
  deleteDialogOpen.value = true
}

const handleDelete = async () => {
  if (!deletingTemplate.value) return

  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const templateId = deletingTemplate.value.id
    const templateName = deletingTemplate.value.workout_Name
    await TemplateApi.deleteWorkoutTemplate(templateId)

    workoutTemplates.value = workoutTemplates.value.filter(workoutTemplate => workoutTemplate.id !== templateId)
    successMessage.value = `Successfully deleted Workout Template "${templateName}".`
    deleteDialogOpen.value = false
    deletingTemplate.value = null
    emit('templates-updated', workoutTemplates.value)
  } catch (error) {
    console.error('Error deleting workout template:', error)
    errorMessage.value = error.message || 'Failed to delete workout template.'
  } finally {
    submitting.value = false
  }
}

// Handle Exercise Template Creation
const handleCreateExerciseTemplate = async (templatePayload) => {
  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const created = await TemplateApi.createExerciseTemplate({
      ...templatePayload,
      userId: activeUser.value
    })
    exerciseTemplates.value.push(created)
    successMessage.value = `Successfully created Exercise Template "${templatePayload.exercise_Name}"!`
    activeTab.value = 'workout_templates'
  } catch (err) {
    console.error('Error creating exercise template:', err)
    errorMessage.value = err.message || 'Failed to create exercise template.'
  } finally {
    submitting.value = false
  }
}

watch(() => props.currentUserId, () => {
  loadData()
})

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="template-view-container" data-testid="template-view">
    <!-- Header banner -->
    <v-card flat elevation="0" class="mb-6 rounded-0 notebook-panel pa-2">
      <v-card-item>
        <div class="d-flex align-center justify-space-between flex-wrap ga-2">
          <div>
            <v-card-title class="text-h6 font-weight-bold d-flex align-center">
              <v-icon icon="mdi-notebook-edit-outline" class="mr-2" color="primary"></v-icon>
              Template View
            </v-card-title>
            <v-card-subtitle>
              Create, edit, and delete workout templates. Any active session must follow a template.
            </v-card-subtitle>
          </div>

          <v-chip
            color="primary"
            variant="tonal"
            size="small"
            class="rounded-0 font-weight-bold"
            data-testid="current-athlete-chip"
          >
            <v-icon icon="mdi-account" start size="x-small"></v-icon>
            Athlete: {{ activeUser }}
          </v-chip>
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

    <!-- Tabs inside Template View -->
    <v-tabs v-model="activeTab" color="primary" class="mb-4 border-b">
      <v-tab value="workout_templates" data-testid="tab-sub-workout-templates" class="rounded-0">
        <v-icon icon="mdi-notebook-outline" start></v-icon>
        Workout Templates ({{ userWorkoutTemplates.length }})
      </v-tab>
      <v-tab value="exercise_templates" data-testid="tab-sub-exercise-templates" class="rounded-0">
        <v-icon icon="mdi-dumbbell" start></v-icon>
        Exercise Templates ({{ exerciseTemplates.length }})
      </v-tab>
      <v-tab value="create_exercise" data-testid="tab-sub-create-exercise" class="rounded-0">
        <v-icon icon="mdi-plus-box-outline" start></v-icon>
        New Exercise Template
      </v-tab>
    </v-tabs>

    <v-window v-model="activeTab">
      <!-- SUB-TAB 1: WORKOUT TEMPLATES (Create, Edit, Delete) -->
      <v-window-item value="workout_templates">
        <!-- CREATE Form -->
        <v-card flat elevation="0" class="mb-6 rounded-0 notebook-panel pa-2">
          <v-card-item>
            <v-card-title class="text-subtitle-1 font-weight-bold text-primary">
              <v-icon icon="mdi-plus-circle" class="mr-1"></v-icon>
              Create New Workout Template
            </v-card-title>
            <v-card-subtitle>
              Build a multi-exercise template to govern active workout sessions.
            </v-card-subtitle>
          </v-card-item>

          <v-card-text>
            <v-form @submit.prevent="handleCreateWorkoutTemplate">
              <v-row align="center">
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="newWorkoutName"
                    data-testid="workout-template-name-input"
                    label="Workout Template Name"
                    placeholder="e.g. Upper Body Hypertrophy"
                    variant="outlined"
                    density="compact"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="12" md="6">
                  <v-select
                    v-model="selectedExerciseIds"
                    data-testid="workout-template-exercises-select"
                    :items="exerciseTemplates"
                    item-title="exercise_Name"
                    item-value="id"
                    label="Select Included Exercises"
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
                  :disabled="!newWorkoutName.trim() || selectedExerciseIds.length === 0"
                >
                  Create Workout Template
                </v-btn>
              </div>
            </v-form>
          </v-card-text>
        </v-card>

        <!-- LIST OF WORKOUT TEMPLATES (With Edit and Delete buttons) -->
        <div class="text-subtitle-1 font-weight-bold mb-3 d-flex align-center justify-space-between">
          <span>Available Workout Templates ({{ userWorkoutTemplates.length }})</span>
          <v-btn
            size="small"
            variant="text"
            icon="mdi-refresh"
            class="rounded-0"
            :loading="loading"
            @click="loadData"
          ></v-btn>
        </div>

        <div v-if="userWorkoutTemplates.length === 0" class="text-center py-8 text-grey">
          <v-icon icon="mdi-text-box-remove-outline" size="48" class="d-block mx-auto mb-2"></v-icon>
          No workout templates exist for {{ activeUser }}. Create one above!
        </div>

        <v-row v-else>
          <v-col
            v-for="wt in userWorkoutTemplates"
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
                <div class="d-flex align-center ga-1">
                  <v-chip size="x-small" color="secondary" variant="flat" class="rounded-0">
                    {{ wt.exerciseTemplates ? wt.exerciseTemplates.length : 0 }} Exercises
                  </v-chip>
                  <!-- EDIT BUTTON -->
                  <v-btn
                    size="small"
                    variant="text"
                    icon="mdi-pencil"
                    color="primary"
                    class="rounded-0"
                    data-testid="edit-workout-template-btn"
                    title="Edit Workout Template"
                    @click="openEditDialog(wt)"
                  ></v-btn>
                  <!-- DELETE BUTTON -->
                  <v-btn
                    size="small"
                    variant="text"
                    icon="mdi-delete"
                    color="error"
                    class="rounded-0"
                    data-testid="delete-workout-template-btn"
                    title="Delete Workout Template"
                    @click="confirmDelete(wt)"
                  ></v-btn>
                </div>
              </div>

              <v-divider class="my-2"></v-divider>

              <div class="text-caption font-weight-bold text-on-surface-variant mb-1">
                Prescribed Exercises:
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
                  <v-list-item-subtitle class="text-caption text-on-surface-variant">
                    {{ et.min_Set }}-{{ et.max_Set }} sets &bull; {{ et.setTemplates ? et.setTemplates.length : 0 }} set specs
                  </v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-card>
          </v-col>
        </v-row>
      </v-window-item>

      <!-- SUB-TAB 2: EXERCISE TEMPLATES (View) -->
      <v-window-item value="exercise_templates">
        <ExerciseTemplateList
          :users="users"
          :templates="exerciseTemplates"
          :loading="loading"
          :current-user-id="activeUser"
          @refresh-templates="loadData"
        />
      </v-window-item>

      <!-- SUB-TAB 3: CREATE EXERCISE TEMPLATE -->
      <v-window-item value="create_exercise">
        <CreateTemplateMenu
          :users="users"
          :submitting="submitting"
          :current-user-id="activeUser"
          @create-template="handleCreateExerciseTemplate"
        />
      </v-window-item>
    </v-window>

    <!-- EDIT WORKOUT TEMPLATE DIALOG -->
    <v-dialog v-model="editDialogOpen" max-width="600px">
      <v-card flat elevation="0" class="pa-4 rounded-0 notebook-panel">
        <v-card-title class="text-h6 font-weight-bold">
          <v-icon icon="mdi-pencil" class="mr-2" color="primary"></v-icon>
          Edit Workout Template
        </v-card-title>
        <v-card-text>
          <v-text-field
            v-model="editWorkoutName"
            data-testid="edit-template-name-input"
            label="Template Name"
            variant="outlined"
            density="compact"
            class="mb-3"
          ></v-text-field>
          <v-select
            v-model="editSelectedExerciseIds"
            data-testid="edit-template-exercises-select"
            :items="exerciseTemplates"
            item-title="exercise_Name"
            item-value="id"
            label="Included Exercise Templates"
            variant="outlined"
            density="compact"
            multiple
            chips
            closable-chips
          ></v-select>
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" class="rounded-0" @click="editDialogOpen = false">Cancel</v-btn>
          <v-btn
            color="primary"
            variant="flat"
            class="rounded-0"
            data-testid="save-edit-template-btn"
            :loading="submitting"
            :disabled="!editWorkoutName.trim() || editSelectedExerciseIds.length === 0"
            @click="handleSaveEdit"
          >
            Save Changes
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- DELETE CONFIRMATION DIALOG -->
    <v-dialog v-model="deleteDialogOpen" max-width="450px">
      <v-card flat elevation="0" class="pa-4 rounded-0 notebook-panel">
        <v-card-title class="text-h6 font-weight-bold text-error">
          <v-icon icon="mdi-alert-circle" class="mr-2" color="error"></v-icon>
          Delete Workout Template?
        </v-card-title>
        <v-card-text>
          Are you sure you want to delete
          <strong>"{{ deletingTemplate?.workout_Name }}"</strong>?
          This action cannot be undone.
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" class="rounded-0" @click="deleteDialogOpen = false">Cancel</v-btn>
          <v-btn
            color="error"
            variant="flat"
            class="rounded-0"
            data-testid="confirm-delete-template-btn"
            :loading="submitting"
            @click="handleDelete"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<style scoped>
*, *::before, *::after {
  border-radius: 0 !important;
}

.template-view-container {
  max-width: 1000px;
  margin: 0 auto;
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

.unrounded-alert {
  border: 1.5px solid #1f1d18 !important;
  box-shadow: none !important;
}
</style>

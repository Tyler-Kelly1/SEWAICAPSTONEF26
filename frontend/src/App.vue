<script setup>
import { ref, onMounted } from 'vue'
import TestTableApi from './services/TestTable.api.js'
import WorkoutSessionApi from './services/WorkoutSession.api.js'
import WorkoutSessionList from './components/WorkoutSessionList.vue'

const activeTab = ref('workouts')

// TEST_TABLE State
const newValue = ref('')
const items = ref([])
const fetchingTable = ref(false)
const submittingTable = ref(false)

// Workout Sessions State
const users = ref([])
const sessions = ref([])
const fetchingWorkouts = ref(false)

const errorMessage = ref('')
const successMessage = ref('')

const fetchTestTableData = async () => {
  fetchingTable.value = true
  errorMessage.value = ''
  try {
    const data = await TestTableApi.getTestItems()
    items.value = data
  } catch (error) {
    console.error('Error fetching TEST_TABLE data:', error)
    errorMessage.value = error.message || 'Failed to load data from API. Make sure the backend is running.'
  } finally {
    fetchingTable.value = false
  }
}

const fetchWorkoutData = async () => {
  fetchingWorkouts.value = true
  errorMessage.value = ''
  try {
    const [usersData, sessionsData] = await Promise.all([
      WorkoutSessionApi.getUsers(),
      WorkoutSessionApi.getSessions()
    ])
    users.value = usersData
    sessions.value = sessionsData
  } catch (error) {
    console.error('Error fetching Workout data:', error)
    errorMessage.value = error.message || 'Failed to load workout session data from API.'
  } finally {
    fetchingWorkouts.value = false
  }
}

const submitValue = async () => {
  if (!newValue.value || !newValue.value.trim()) return

  submittingTable.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    await TestTableApi.createTestItem(newValue.value.trim())
    successMessage.value = `Successfully inserted "${newValue.value.trim()}" into TEST_TABLE!`
    newValue.value = ''
    await fetchTestTableData()
  } catch (error) {
    console.error('Error inserting row into TEST_TABLE:', error)
    errorMessage.value = error.message || 'Failed to insert row. Please try again.'
  } finally {
    submittingTable.value = false
  }
}

const formatDate = (isoString) => {
  if (!isoString) return ''
  return new Date(isoString).toLocaleString()
}

onMounted(() => {
  fetchTestTableData()
  fetchWorkoutData()
})
</script>

<template>
  <v-app theme="dark">
    <v-app-bar color="primary" elevation="2">
      <v-app-bar-title class="font-weight-bold d-flex align-center">
        <v-icon icon="mdi-dumbbell" class="mr-2"></v-icon>
        SEW AI Capstone - Overload Data Explorer
      </v-app-bar-title>
    </v-app-bar>

    <v-main class="bg-grey-darken-4">
      <v-container class="py-8" style="max-width: 950px;">
        
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

        <!-- Navigation Tabs -->
        <v-tabs
          v-model="activeTab"
          color="primary"
          align-tabs="start"
          class="mb-6 border-b"
        >
          <v-tab value="workouts">
            <v-icon icon="mdi-dumbbell" start></v-icon>
            Workout Sessions
          </v-tab>
          <v-tab value="test_table">
            <v-icon icon="mdi-table-large" start></v-icon>
            TEST_TABLE Demo
          </v-tab>
        </v-tabs>

        <!-- Tab 1: Workout Sessions (Seeded Data Models) -->
        <v-window v-model="activeTab">
          <v-window-item value="workouts">
            <WorkoutSessionList
              :users="users"
              :sessions="sessions"
              :loading="fetchingWorkouts"
              @refresh="fetchWorkoutData"
            />
          </v-window-item>

          <!-- Tab 2: Original TEST_TABLE -->
          <v-window-item value="test_table">
            <v-card class="mb-6 rounded-lg elevation-4 pa-2">
              <v-card-item>
                <v-card-title class="text-h6 font-weight-bold">
                  <v-icon icon="mdi-plus-circle-outline" class="mr-2" color="primary"></v-icon>
                  Add Row to TEST_TABLE
                </v-card-title>
                <v-card-subtitle>
                  Enter a value below to save it to PostgreSQL via the .NET API controller.
                </v-card-subtitle>
              </v-card-item>

              <v-card-text>
                <v-form @submit.prevent="submitValue">
                  <v-row align="center">
                    <v-col cols="12" sm="9">
                      <v-text-field
                        v-model="newValue"
                        label="Value"
                        placeholder="Enter data here..."
                        variant="outlined"
                        prepend-inner-icon="mdi-pencil"
                        clearable
                        hide-details="auto"
                        :disabled="submittingTable"
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="3">
                      <v-btn
                        color="primary"
                        block
                        size="large"
                        type="submit"
                        :loading="submittingTable"
                        :disabled="!newValue || !newValue.trim()"
                        prepend-icon="mdi-send"
                      >
                        Submit
                      </v-btn>
                    </v-col>
                  </v-row>
                </v-form>
              </v-card-text>
            </v-card>

            <v-card class="rounded-lg elevation-4 pa-2">
              <v-card-item>
                <div class="d-flex align-center justify-space-between">
                  <div>
                    <v-card-title class="text-h6 font-weight-bold">
                      <v-icon icon="mdi-table-large" class="mr-2" color="secondary"></v-icon>
                      TEST_TABLE Records
                    </v-card-title>
                    <v-card-subtitle>
                      Total rows: {{ items.length }}
                    </v-card-subtitle>
                  </div>
                  <v-btn
                    icon="mdi-refresh"
                    variant="text"
                    color="primary"
                    :loading="fetchingTable"
                    @click="fetchTestTableData"
                  ></v-btn>
                </div>
              </v-card-item>

              <v-divider></v-divider>

              <v-card-text class="pa-0">
                <v-list lines="two" bg-color="transparent" class="py-0">
                  <template v-if="items.length > 0">
                    <template v-for="(item, index) in items" :key="item.id">
                      <v-list-item class="py-3">
                        <template v-slot:prepend>
                          <v-avatar color="primary" variant="tonal">
                            <v-icon icon="mdi-file-document-outline"></v-icon>
                          </v-avatar>
                        </template>

                        <v-list-item-title class="text-subtitle-1 font-weight-bold">
                          {{ item.value }}
                        </v-list-item-title>

                        <v-list-item-subtitle class="text-caption text-grey-lighten-1 mt-1">
                          <v-icon icon="mdi-clock-outline" size="x-small" class="mr-1"></v-icon>
                          INSERT_TIME_STAMP: {{ formatDate(item.insertTimeStamp) }}
                        </v-list-item-subtitle>

                        <template v-slot:append>
                          <v-chip size="small" variant="outlined" color="primary">
                            ID: {{ item.id }}
                          </v-chip>
                        </template>
                      </v-list-item>
                      <v-divider v-if="index < items.length - 1"></v-divider>
                    </template>
                  </template>

                  <v-list-item v-else-if="!fetchingTable" class="text-center py-6">
                    <v-list-item-title class="text-grey-lighten-1">
                      <v-icon icon="mdi-inbox-remove" size="large" class="d-block mx-auto mb-2" color="grey"></v-icon>
                      No rows found in TEST_TABLE. Submit a value above to insert one!
                    </v-list-item-title>
                  </v-list-item>

                  <v-list-item v-else class="text-center py-6">
                    <v-progress-circular indeterminate color="primary"></v-progress-circular>
                  </v-list-item>
                </v-list>
              </v-card-text>
            </v-card>
          </v-window-item>
        </v-window>

      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
</style>

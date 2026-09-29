<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  users: {
    type: Array,
    default: () => []
  },
  templates: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  },
  currentUserId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['user-change', 'refresh-templates'])

const selectedUser = ref(props.users.length > 0 ? props.users[0].userId : 'tyler_dev')

const handleUserChange = (newUserId) => {
  selectedUser.value = newUserId
  emit('user-change', newUserId)
}

const activeUser = computed(() => props.currentUserId || selectedUser.value)

const filteredTemplates = computed(() => {
  if (!activeUser.value) return props.templates
  return props.templates.filter(template => !template.userId || template.userId === activeUser.value)
})
</script>

<template>
  <v-card flat elevation="0" class="rounded-0 notebook-panel pa-2">
    <v-card-item>
      <div class="d-flex align-center justify-space-between flex-wrap ga-2">
        <div>
          <v-card-title class="text-h6 font-weight-bold">
            <v-icon icon="mdi-file-document-outline" class="mr-2" color="primary"></v-icon>
            User Exercise Templates
          </v-card-title>
          <v-card-subtitle>
            View all exercise templates configured for the active user
          </v-card-subtitle>
        </div>

        <div class="d-flex align-center ga-2">
          <v-chip
            v-if="currentUserId"
            color="primary"
            variant="tonal"
            size="small"
            class="rounded-0 font-weight-bold"
            data-testid="exercise-list-user-chip"
          >
            <v-icon icon="mdi-account" start size="x-small"></v-icon>
            Athlete: {{ currentUserId }}
          </v-chip>
          <v-select
            v-else-if="users.length > 0"
            :model-value="selectedUser"
            :items="users.map(u => u.userId)"
            label="Select User"
            variant="outlined"
            density="compact"
            hide-details="auto"
            style="min-width: 180px;"
            @update:model-value="handleUserChange"
          ></v-select>

          <v-btn
            icon="mdi-refresh"
            variant="text"
            color="primary"
            class="rounded-0"
            :loading="loading"
            @click="emit('refresh-templates')"
          ></v-btn>
        </div>
      </div>
    </v-card-item>

    <v-divider class="my-2"></v-divider>

    <v-card-text>
      <div v-if="loading" class="text-center py-8">
        <v-progress-circular indeterminate color="primary" size="48"></v-progress-circular>
        <div class="text-caption text-grey mt-2">Loading exercise templates...</div>
      </div>

      <div v-else-if="filteredTemplates.length === 0" class="text-center py-8">
        <v-icon icon="mdi-folder-open-outline" size="48" color="grey"></v-icon>
        <div class="text-subtitle-1 text-grey mt-2">No exercise templates found for {{ activeUser }}</div>
        <div class="text-caption text-grey-lighten-1">Use the "Create Template" menu to create one!</div>
      </div>

      <v-row v-else>
        <v-col
          v-for="template in filteredTemplates"
          :key="template.id || template.exercise_Name"
          cols="12"
          md="6"
        >
          <v-card flat elevation="0" class="rounded-0 notebook-subpanel pa-3 fill-height">
            <div class="d-flex align-center justify-space-between mb-2">
              <div class="text-subtitle-1 font-weight-bold text-primary">
                <v-icon icon="mdi-weight-lifter" class="mr-1" size="small"></v-icon>
                {{ template.exercise_Name }}
              </div>
              <v-chip size="x-small" color="primary" class="rounded-0">
                {{ template.setTemplates ? template.setTemplates.length : 0 }} Set Templates
              </v-chip>
            </div>

            <v-divider class="mb-3"></v-divider>

            <div class="text-caption text-on-surface-variant mb-2">
              <div><strong>Sets Target:</strong> {{ template.min_Set }} - {{ template.max_Set }} sets</div>
              <div><strong>Weight Step:</strong> {{ ((template.weight_Step || 0.05) * 100).toFixed(0) }}%</div>
              <div><strong>Volume Step:</strong> +{{ template.volume_Step || 1 }} rep(s)</div>
            </div>

            <div class="text-subtitle-2 font-weight-bold mt-3 mb-1 text-on-surface-variant">
              Set Prescriptions:
            </div>
            
            <v-table density="compact" bg-color="transparent" class="rounded-0 notebook-table">
              <thead>
                <tr>
                  <th class="text-left text-caption">Set #</th>
                  <th class="text-left text-caption">Target Reps</th>
                  <th class="text-left text-caption">Type</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(setTemp, idx) in template.setTemplates || []" :key="setTemp.id || idx">
                  <td class="text-caption font-weight-bold">Set {{ idx + 1 }}</td>
                  <td class="text-caption">{{ setTemp.min_Reps }} - {{ setTemp.max_Reps }} reps</td>
                  <td class="text-caption">
                    <v-chip
                      size="x-small"
                      :color="setTemp.failure_Set ? 'error' : 'success'"
                      variant="tonal"
                      class="rounded-0"
                    >
                      {{ setTemp.failure_Set ? 'Failure' : 'Standard' }}
                    </v-chip>
                  </td>
                </tr>
              </tbody>
            </v-table>
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
</style>

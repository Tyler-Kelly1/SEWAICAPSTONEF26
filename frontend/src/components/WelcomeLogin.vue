<script setup>
import { ref } from 'vue'

const props = defineProps({
  existingUsers: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['check-user', 'create-user', 'login-success'])

const inputUserId = ref('')
const isChecking = ref(false)
const isCreating = ref(false)
const userNotFound = ref(false)
const pendingUserId = ref('')
const errorMessage = ref('')
const infoMessage = ref('')

const handleContinue = () => {
  errorMessage.value = ''
  infoMessage.value = ''
  userNotFound.value = false

  const trimmed = inputUserId.value?.trim()
  if (!trimmed) {
    errorMessage.value = 'Please enter a User ID.'
    return
  }

  isChecking.value = true
  emit('check-user', {
    userId: trimmed,
    callback: ({ exists, userId, error }) => {
      isChecking.value = false
      if (error) {
        errorMessage.value = error
        return
      }

      if (exists) {
        emit('login-success', userId)
      } else {
        userNotFound.value = true
        pendingUserId.value = userId || trimmed
      }
    }
  })
}

const handleCreateAccount = () => {
  if (!pendingUserId.value) return

  isCreating.value = true
  errorMessage.value = ''

  emit('create-user', {
    userId: pendingUserId.value,
    callback: ({ success, userId, error }) => {
      isCreating.value = false
      if (error) {
        errorMessage.value = error
        return
      }

      if (success) {
        emit('login-success', userId || pendingUserId.value)
      }
    }
  })
}

const resetNotFound = () => {
  userNotFound.value = false
  pendingUserId.value = ''
  errorMessage.value = ''
}

const selectExistingUser = (userId) => {
  inputUserId.value = userId
  handleContinue()
}
</script>

<template>
  <div class="login-dossier-frame">
    <!-- Main Dossier Content -->
    <div class="pa-6 bg-surface">
      <div class="mb-4 pb-3 border-b border-outline-variant">
        <h1 class="font-headline text-h5 font-weight-bold text-primary text-uppercase mb-1">
          Welcome to Overload
        </h1>
        <p class="font-body text-body-2 text-on-surface-variant mb-0">
          Enter your User ID to start tracking sessions and progressive overload.
        </p>
      </div>

      <!-- Error Alert -->
      <div
        v-if="errorMessage"
        class="alert-brutalist alert-error mb-4 pa-3 d-flex align-center justify-space-between font-mono text-caption font-weight-bold"
        data-testid="login-error-alert"
      >
        <div class="d-flex align-center ga-2">
          <v-icon icon="mdi-alert-outline" size="18" color="error"></v-icon>
          <span>{{ errorMessage }}</span>
        </div>
        <button type="button" class="close-alert-btn" @click="errorMessage = ''">&times;</button>
      </div>

      <!-- Info Alert -->
      <div
        v-if="infoMessage"
        class="alert-brutalist alert-info mb-4 pa-3 d-flex align-center justify-space-between font-mono text-caption font-weight-bold"
      >
        <div class="d-flex align-center ga-2">
          <v-icon icon="mdi-information-outline" size="18" color="primary"></v-icon>
          <span>{{ infoMessage }}</span>
        </div>
        <button type="button" class="close-alert-btn" @click="infoMessage = ''">&times;</button>
      </div>

      <!-- User Not Found Prompt / Account Creation Confirmation -->
      <div
        v-if="userNotFound"
        class="account-prompt-box pa-4 mb-5"
        data-testid="user-not-found-prompt"
      >
        <div class="d-flex align-center ga-2 mb-2">
          <span class="font-mono text-caption font-weight-bold text-error uppercase">[UNREGISTERED ATHLETE]</span>
        </div>
        <div class="font-headline text-subtitle-1 font-weight-bold text-primary mb-1">
          User "{{ pendingUserId }}" does not exist in local registry.
        </div>
        <div class="font-body text-body-2 text-on-surface-variant mb-4">
          Would you like to initialize a new athlete profile with this User ID? No password is required.
        </div>

        <div class="d-flex flex-wrap ga-3">
          <button
            type="button"
            class="brutalist-btn brutalist-btn-primary"
            :disabled="isCreating"
            data-testid="btn-create-account"
            @click="handleCreateAccount"
          >
            <span v-if="isCreating">INITIALIZING...</span>
            <span v-else>[ + INITIALIZE ACCOUNT ]</span>
          </button>
          <button
            type="button"
            class="brutalist-btn brutalist-btn-secondary"
            :disabled="isCreating"
            @click="resetNotFound"
          >
            [ CANCEL ]
          </button>
        </div>
      </div>

      <!-- Main Login Input Form -->
      <form v-if="!userNotFound" @submit.prevent="handleContinue">
        <div class="mb-4">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            ATHLETE USER ID / LOG IDENTIFIER:
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">✏️</span>
            <input
              v-model="inputUserId"
              type="text"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="Enter your user ID (e.g. tyler_dev)"
              autofocus
              :disabled="isChecking || loading"
              data-testid="input-user-id"
            />
          </div>
        </div>

        <button
          type="submit"
          class="brutalist-btn brutalist-btn-primary w-100 py-3"
          :disabled="isChecking || loading || !inputUserId || !inputUserId.trim()"
          data-testid="btn-login-submit"
        >
          <span v-if="isChecking || loading">AUTHENTICATING...</span>
          <span v-else>[ CONTINUE &rarr; ]</span>
        </button>
      </form>

      <!-- Quick Select Registered Users -->
      <div v-if="existingUsers && existingUsers.length > 0" class="mt-5 pt-3 border-t border-outline-variant">
        <div class="font-mono text-caption text-on-surface-variant font-weight-bold uppercase mb-2">
          // RECENT ATHLETE PROFILES:
        </div>
        <div class="d-flex flex-wrap ga-2">
          <button
            v-for="user in existingUsers"
            :key="user.userId || user"
            type="button"
            class="user-profile-tag font-mono text-caption font-weight-bold"
            @click="selectExistingUser(user.userId || user)"
          >
            <span class="text-outline mr-1">#</span>{{ user.userId || user }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Void Rounding - Strict 0px Corners Across the Entire Component */
*, *::before, *::after {
  border-radius: 0 !important;
}

.login-dossier-frame {
  width: 100%;
  max-width: 580px;
  margin: 0 auto;
  background-color: #f6ebd0; /* surface from DESIGN.md */
  border: 2px solid #1f1d18; /* primary from DESIGN.md */
  box-shadow: none;           /* No shadow on card */
}

.input-well {
  background-color: #fcf5dc; /* surface-container-lowest from DESIGN.md */
  border: 2px solid #1f1d18;
  box-shadow: 2px 2px 0px 0px #2c2820;
}

.unrounded-input {
  border: none;
  outline: none;
  background: transparent;
  color: #1f1d18;
}

.unrounded-input::placeholder {
  color: #786f58; /* outline from DESIGN.md */
  font-family: 'Patrick Hand', cursive, sans-serif;
}

/* Brutalist Button System */
.brutalist-btn {
  font-family: 'Patrick Hand', cursive, sans-serif;
  font-weight: 700;
  letter-spacing: 0.05em;
  padding: 0.6rem 1.2rem;
  border: 2px solid #1f1d18;
  cursor: pointer;
  transition: transform 0.06s ease, box-shadow 0.06s ease;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.brutalist-btn-primary {
  background-color: #1f1d18; /* primary from DESIGN.md */
  color: #fefbe8;            /* on-primary from DESIGN.md */
  box-shadow: 2px 2px 0px 0px #2c2820;
}

.brutalist-btn-secondary {
  background-color: #f0e1b9; /* surface-container from DESIGN.md */
  color: #1f1d18;
  box-shadow: 2px 2px 0px 0px #2c2820;
}

.brutalist-btn:hover:not(:disabled) {
  opacity: 0.95;
}

.brutalist-btn:active:not(:disabled) {
  transform: translate(2px, 2px);
  box-shadow: 0px 0px 0px 0px transparent !important;
}

.brutalist-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  box-shadow: none;
}

/* Alerts */
.alert-brutalist {
  border: 1.5px solid #1f1d18;
  box-shadow: 2px 2px 0px 0px #2c2820;
}

.alert-error {
  background-color: #fcdad7; /* error-container from DESIGN.md */
  color: #93000a;            /* on-error-container from DESIGN.md */
  border-color: #b91c1c;     /* error from DESIGN.md */
}

.alert-info {
  background-color: #eedea8; /* secondary-container from DESIGN.md */
  color: #1f1d18;
}

.close-alert-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  line-height: 1;
  cursor: pointer;
  color: inherit;
}

/* Account creation prompt */
.account-prompt-box {
  background-color: #f4e7c5; /* surface-container-low from DESIGN.md */
  border: 1.5px solid #b91c1c;
}

/* Athlete tags */
.user-profile-tag {
  background-color: #eedea8; /* secondary-container from DESIGN.md */
  border: 1.5px solid #1f1d18;
  color: #1f1d18;
  padding: 0.25rem 0.5rem;
  cursor: pointer;
  box-shadow: 1px 1px 0px 0px #2c2820;
  transition: transform 0.08s ease;
}

.user-profile-tag:hover {
  background-color: #ebdcaf;
}

.user-profile-tag:active {
  transform: translate(1px, 1px);
  box-shadow: none;
}
</style>

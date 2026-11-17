<script setup>
import { ref } from 'vue'

const props = defineProps({
  loading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['login', 'register', 'login-success'])

const activeMode = ref('login') // 'login' | 'register'
const isSubmitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Login form model
const loginEmail = ref('')
const loginPassword = ref('')

// Register form model
const registerUsername = ref('')
const registerEmail = ref('')
const registerPassword = ref('')
const registerConfirmPassword = ref('')

const switchMode = (mode) => {
  activeMode.value = mode
  errorMessage.value = ''
  successMessage.value = ''
}

const handleLoginSubmit = () => {
  errorMessage.value = ''
  successMessage.value = ''

  const emailTrimmed = loginEmail.value?.trim()
  if (!emailTrimmed) {
    errorMessage.value = 'Please enter your registered email address.'
    return
  }

  if (!loginPassword.value) {
    errorMessage.value = 'Please enter your password.'
    return
  }

  isSubmitting.value = true
  emit('login', {
    email: emailTrimmed,
    password: loginPassword.value,
    callback: ({ success, user, error }) => {
      isSubmitting.value = false
      if (error) {
        errorMessage.value = error
        return
      }
      if (success) {
        emit('login-success', user?.userId || user?.userName || emailTrimmed)
      }
    }
  })
}

const handleRegisterSubmit = () => {
  errorMessage.value = ''
  successMessage.value = ''

  const usernameTrimmed = registerUsername.value?.trim()
  const emailTrimmed = registerEmail.value?.trim()

  if (!usernameTrimmed) {
    errorMessage.value = 'Please choose an athlete username.'
    return
  }

  if (!emailTrimmed) {
    errorMessage.value = 'Please enter a valid email address.'
    return
  }

  if (!registerPassword.value || registerPassword.value.length < 6) {
    errorMessage.value = 'Password must be at least 6 characters long.'
    return
  }

  if (registerPassword.value !== registerConfirmPassword.value) {
    errorMessage.value = 'Passwords do not match.'
    return
  }

  isSubmitting.value = true
  emit('register', {
    username: usernameTrimmed,
    email: emailTrimmed,
    password: registerPassword.value,
    callback: ({ success, user, error }) => {
      isSubmitting.value = false
      if (error) {
        errorMessage.value = error
        return
      }
      if (success) {
        successMessage.value = 'Account created successfully! Authenticating...'
        emit('login-success', user?.userId || user?.userName || usernameTrimmed)
      }
    }
  })
}
</script>

<template>
  <div class="login-dossier-frame">
    <!-- Main Dossier Content -->
    <div class="pa-6 bg-surface">
      <!-- Dossier Header -->
      <div class="mb-4 pb-3 border-b border-outline-variant">
        <h1 class="font-headline text-h5 font-weight-bold text-primary text-uppercase mb-1">
          Welcome to Overload
        </h1>
        <p class="font-body text-body-2 text-on-surface-variant mb-0">
          Analog Gym Journal & Progressive Overload Tracking. Authenticate with your email and password.
        </p>
      </div>

      <!-- Mode Switcher Tabs -->
      <div class="d-flex ga-2 mb-4">
        <button
          type="button"
          class="brutalist-tab-btn"
          :class="{ 'brutalist-tab-btn--active': activeMode === 'login' }"
          data-testid="tab-login-mode"
          @click="switchMode('login')"
        >
          [ LOG IN ]
        </button>
        <button
          type="button"
          class="brutalist-tab-btn"
          :class="{ 'brutalist-tab-btn--active': activeMode === 'register' }"
          data-testid="tab-register-mode"
          @click="switchMode('register')"
        >
          [ REGISTER ACCOUNT ]
        </button>
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

      <!-- Success Alert -->
      <div
        v-if="successMessage"
        class="alert-brutalist alert-info mb-4 pa-3 d-flex align-center justify-space-between font-mono text-caption font-weight-bold"
      >
        <div class="d-flex align-center ga-2">
          <v-icon icon="mdi-check-circle-outline" size="18" color="primary"></v-icon>
          <span>{{ successMessage }}</span>
        </div>
        <button type="button" class="close-alert-btn" @click="successMessage = ''">&times;</button>
      </div>

      <!-- LOGIN FORM -->
      <form v-if="activeMode === 'login'" data-testid="form-login" @submit.prevent="handleLoginSubmit">
        <div class="mb-3">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            ATHLETE EMAIL ADDRESS:
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">✉️</span>
            <input
              v-model="loginEmail"
              type="email"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="e.g. tyler@example.com"
              autofocus
              :disabled="isSubmitting || loading"
              data-testid="input-login-email"
            />
          </div>
        </div>

        <div class="mb-4">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            PASSWORD:
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">🔒</span>
            <input
              v-model="loginPassword"
              type="password"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="Enter your password"
              :disabled="isSubmitting || loading"
              data-testid="input-login-password"
            />
          </div>
        </div>

        <button
          type="submit"
          class="brutalist-btn brutalist-btn-primary w-100 py-3"
          :disabled="isSubmitting || loading || !loginEmail || !loginPassword"
          data-testid="btn-login-submit"
        >
          <span v-if="isSubmitting || loading">AUTHENTICATING...</span>
          <span v-else>[ CONTINUE &rarr; ]</span>
        </button>
      </form>

      <!-- REGISTER FORM -->
      <form v-else data-testid="form-register" @submit.prevent="handleRegisterSubmit">
        <div class="mb-3">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            CHOOSE ATHLETE USERNAME / LOG ID:
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">👤</span>
            <input
              v-model="registerUsername"
              type="text"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="e.g. tyler_dev"
              autofocus
              :disabled="isSubmitting || loading"
              data-testid="input-register-username"
            />
          </div>
        </div>

        <div class="mb-3">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            EMAIL ADDRESS:
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">✉️</span>
            <input
              v-model="registerEmail"
              type="email"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="e.g. tyler@example.com"
              :disabled="isSubmitting || loading"
              data-testid="input-register-email"
            />
          </div>
        </div>

        <div class="mb-3">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            CREATE PASSWORD (MIN 6 CHARACTERS):
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">🔒</span>
            <input
              v-model="registerPassword"
              type="password"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="Enter password"
              :disabled="isSubmitting || loading"
              data-testid="input-register-password"
            />
          </div>
        </div>

        <div class="mb-4">
          <label class="d-block font-mono text-caption font-weight-bold text-primary uppercase mb-1">
            CONFIRM PASSWORD:
          </label>
          <div class="input-well d-flex align-center px-3 py-2">
            <span class="mr-2 font-mono text-outline font-weight-bold">🔒</span>
            <input
              v-model="registerConfirmPassword"
              type="password"
              class="font-mono text-body-1 text-primary unrounded-input w-100"
              placeholder="Re-enter password"
              :disabled="isSubmitting || loading"
              data-testid="input-register-confirm-password"
            />
          </div>
        </div>

        <button
          type="submit"
          class="brutalist-btn brutalist-btn-primary w-100 py-3"
          :disabled="isSubmitting || loading || !registerUsername || !registerEmail || !registerPassword"
          data-testid="btn-register-submit"
        >
          <span v-if="isSubmitting || loading">INITIALIZING ATHLETE ACCOUNT...</span>
          <span v-else>[ + INITIALIZE ACCOUNT ]</span>
        </button>
      </form>
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

/* Tab Switcher */
.brutalist-tab-btn {
  flex: 1;
  font-family: 'Patrick Hand', cursive, sans-serif;
  font-weight: 700;
  letter-spacing: 0.05em;
  padding: 0.5rem 1rem;
  border: 2px solid #1f1d18;
  background-color: #f0e1b9; /* surface-container */
  color: #1f1d18;
  cursor: pointer;
  box-shadow: 2px 2px 0px 0px #2c2820;
  transition: all 0.08s ease;
}

.brutalist-tab-btn--active {
  background-color: #1f1d18;
  color: #fefbe8;
  box-shadow: none;
  transform: translate(2px, 2px);
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
</style>

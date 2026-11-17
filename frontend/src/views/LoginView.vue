<script setup>
import { ref } from 'vue'
import AuthApi from '../services/Auth.api.js'
import WelcomeLogin from '../components/WelcomeLogin.vue'
import { useHashRouter } from '../router/index.js'

const props = defineProps({
  currentUserId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['login-success'])

const router = useHashRouter()
const isAuthenticating = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const handleLogin = async ({ email, password, callback }) => {
  errorMessage.value = ''
  isAuthenticating.value = true
  try {
    const user = await AuthApi.login({ email, password })
    callback({ success: true, user })
  } catch (err) {
    callback({ error: err.message || 'Login failed.' })
  } finally {
    isAuthenticating.value = false
  }
}

const handleRegister = async ({ username, email, password, callback }) => {
  errorMessage.value = ''
  isAuthenticating.value = true
  try {
    const registered = await AuthApi.register({ username, email, password })
    // Automatically log in following registration
    const user = await AuthApi.login({ email, password })
    callback({ success: true, user })
  } catch (err) {
    callback({ error: err.message || 'Registration failed.' })
  } finally {
    isAuthenticating.value = false
  }
}

const handleLoginSuccess = (userId) => {
  emit('login-success', userId)
  router.navigateTo('/session')
}
</script>

<template>
  <div class="login-view-grounded py-8" data-testid="login-view">
    <v-alert
      v-if="errorMessage"
      type="error"
      variant="tonal"
      closable
      class="mb-4 mx-auto unrounded-alert font-mono"
      max-width="580"
      @click:close="errorMessage = ''"
    >
      {{ errorMessage }}
    </v-alert>

    <v-alert
      v-if="successMessage"
      type="success"
      variant="tonal"
      closable
      class="mb-4 mx-auto unrounded-alert font-mono"
      max-width="580"
      @click:close="successMessage = ''"
    >
      {{ successMessage }}
    </v-alert>

    <WelcomeLogin
      :loading="isAuthenticating"
      @login="handleLogin"
      @register="handleRegister"
      @login-success="handleLoginSuccess"
    />
  </div>
</template>

<style scoped>
*, *::before, *::after {
  border-radius: 0 !important;
}

.login-view-grounded {
  width: 100%;
  min-height: 60vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: transparent;
}

.unrounded-alert {
  border: 2px solid #1f1d18 !important;
  box-shadow: 2px 2px 0px 0px #2c2820 !important;
  border-radius: 0 !important;
}
</style>

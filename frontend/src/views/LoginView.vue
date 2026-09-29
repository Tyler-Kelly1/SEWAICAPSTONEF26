<script setup>
import { ref, onMounted } from 'vue'
import UserApi from '../services/User.api.js'
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
const users = ref([])
const loadingUsers = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const loadUsers = async () => {
  loadingUsers.value = true
  try {
    const list = await UserApi.getUsers()
    users.value = list || []
  } catch (err) {
    console.warn('Could not load user list:', err)
  } finally {
    loadingUsers.value = false
  }
}

const handleCheckUser = async ({ userId, callback }) => {
  errorMessage.value = ''
  try {
    const result = await UserApi.checkUserExists(userId)
    callback({ exists: result.exists, userId: result.userId || userId })
  } catch (err) {
    console.error('Error verifying user ID:', err)
    callback({ error: err.message || 'Failed to check user ID with backend.' })
  }
}

const handleCreateUser = async ({ userId, callback }) => {
  errorMessage.value = ''
  try {
    const created = await UserApi.createUser({ userId })
    await loadUsers()
    callback({ success: true, userId: created.userId || userId })
  } catch (err) {
    console.error('Error creating user:', err)
    callback({ error: err.message || 'Failed to create user account.' })
  }
}

const handleLoginSuccess = (userId) => {
  if (typeof localStorage !== 'undefined') {
    localStorage.setItem('userId', userId)
    localStorage.setItem('overload_user_id', userId)
  }
  emit('login-success', userId)
  router.navigateTo('/')
}

onMounted(() => {
  loadUsers()
})
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
      :existing-users="users"
      :loading="loadingUsers"
      @check-user="handleCheckUser"
      @create-user="handleCreateUser"
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

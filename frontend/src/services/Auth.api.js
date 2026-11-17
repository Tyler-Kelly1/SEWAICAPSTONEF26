const API_BASE_URL = import.meta.env.VITE_API_URL 
  ? `${import.meta.env.VITE_API_URL.replace(/\/WorkoutSession\/?$/, '')}/Auth`
  : 'http://localhost:5245/api/Auth'

const INACTIVITY_TIMEOUT_MS = 24 * 60 * 60 * 1000 // 24 hours

const AuthApi = {
  recordActivity() {
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('overload_last_activity', Date.now().toString())
    }
  },

  getLastActivity() {
    if (typeof localStorage !== 'undefined') {
      const val = localStorage.getItem('overload_last_activity')
      return val ? parseInt(val, 10) : null
    }
    return null
  },

  isSessionExpired() {
    const last = this.getLastActivity()
    if (!last) return false
    return (Date.now() - last) > INACTIVITY_TIMEOUT_MS
  },

  clearSession() {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('userId')
      localStorage.removeItem('overload_user_id')
      localStorage.removeItem('overload_user_name')
      localStorage.removeItem('overload_user_email')
      localStorage.removeItem('overload_last_activity')
    }
  },

  async register({ username, email, password }) {
    const response = await fetch(`${API_BASE_URL}/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      credentials: 'include',
      body: JSON.stringify({
        userName: username?.trim(),
        email: email?.trim(),
        password: password
      })
    })

    if (!response.ok) {
      let errorMsg = `Registration failed (HTTP ${response.status})`
      try {
        const errorJson = await response.json()
        errorMsg = errorJson.message || errorJson.title || errorMsg
      } catch {
        const text = await response.text()
        if (text) errorMsg = text
      }
      throw new Error(errorMsg)
    }

    const data = await response.json()
    this.recordActivity()
    return data
  },

  async login({ email, password }) {
    const response = await fetch(`${API_BASE_URL}/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      credentials: 'include',
      body: JSON.stringify({
        email: email?.trim(),
        password: password
      })
    })

    if (!response.ok) {
      let errorMsg = 'Invalid email or password.'
      try {
        const errorJson = await response.json()
        errorMsg = errorJson.message || errorJson.title || errorMsg
      } catch {
        const text = await response.text()
        if (text) errorMsg = text
      }
      throw new Error(errorMsg)
    }

    const data = await response.json()
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('userId', data.userId || data.userName)
      localStorage.setItem('overload_user_id', data.userId || data.userName)
      localStorage.setItem('overload_user_name', data.userName || '')
      localStorage.setItem('overload_user_email', data.email || '')
    }
    this.recordActivity()
    return data
  },

  async logout() {
    try {
      await fetch(`${API_BASE_URL}/logout`, {
        method: 'POST',
        credentials: 'include'
      })
    } catch (err) {
      console.warn('Backend logout request failed:', err)
    } finally {
      this.clearSession()
    }
  },

  async getCurrentUser() {
    if (this.isSessionExpired()) {
      this.clearSession()
      throw new Error('Session expired due to 24 hours of inactivity. Please log in again.')
    }

    const response = await fetch(`${API_BASE_URL}/me`, {
      method: 'GET',
      credentials: 'include'
    })

    if (!response.ok) {
      this.clearSession()
      let errorMsg = `Not authenticated (HTTP ${response.status})`
      try {
        const errJson = await response.json()
        errorMsg = errJson.message || errorMsg
      } catch {
        // use default
      }
      throw new Error(errorMsg)
    }

    const user = await response.json()
    this.recordActivity()
    return user
  },

  isAuthenticated() {
    if (this.isSessionExpired()) {
      this.clearSession()
      return false
    }
    if (typeof localStorage === 'undefined') return false
    return !!(localStorage.getItem('userId') || localStorage.getItem('overload_user_id'))
  }
}

export default AuthApi

const API_BASE_URL = import.meta.env.VITE_API_URL 
  ? `${import.meta.env.VITE_API_URL.replace(/\/WorkoutSession\/?$/, '')}/User`
  : 'http://localhost:5245/api/User'

const UserApi = {
  async checkUserExists(userId) {
    if (!userId || !userId.trim()) {
      return { exists: false, userId: '' }
    }
    const cleanId = userId.trim()
    const response = await fetch(`${API_BASE_URL}/exists/${encodeURIComponent(cleanId)}`)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to check user ID (HTTP ${response.status})`)
    }
    return await response.json()
  },

  async getUser(userId) {
    if (!userId || !userId.trim()) {
      throw new Error('User ID is required.')
    }
    const cleanId = userId.trim()
    const response = await fetch(`${API_BASE_URL}/${encodeURIComponent(cleanId)}`)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch user (HTTP ${response.status})`)
    }
    return await response.json()
  },

  async createUser(userData) {
    const userId = typeof userData === 'string' ? userData : userData?.userId
    if (!userId || !userId.trim()) {
      throw new Error('User ID is required to create an account.')
    }

    const response = await fetch(API_BASE_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ userId: userId.trim() })
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to create user (HTTP ${response.status})`)
    }

    return await response.json()
  },

  async getUsers() {
    const response = await fetch(API_BASE_URL)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch users (HTTP ${response.status})`)
    }
    return await response.json()
  }
}

export default UserApi

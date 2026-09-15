const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5245/api/WorkoutSession'

const WorkoutSessionApi = {
  async getUsers() {
    const response = await fetch(`${API_BASE_URL}/users`)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch users (HTTP ${response.status})`)
    }
    return await response.json()
  },

  async getSessions(userId = null) {
    const url = userId ? `${API_BASE_URL}?userId=${encodeURIComponent(userId)}` : API_BASE_URL
    const response = await fetch(url)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch sessions (HTTP ${response.status})`)
    }
    return await response.json()
  },

  async createSession(sessionPayload) {
    const response = await fetch(API_BASE_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(sessionPayload)
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to create session (HTTP ${response.status})`)
    }

    return await response.json()
  }
}

export default WorkoutSessionApi

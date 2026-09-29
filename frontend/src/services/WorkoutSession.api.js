const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5245/api/WorkoutSession'

const getStoredUserId = (userId = null) => {
  if (userId) return userId
  if (typeof localStorage !== 'undefined') {
    return localStorage.getItem('userId') || localStorage.getItem('overload_user_id') || null
  }
  return null
}

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
    const activeUserId = getStoredUserId(userId)
    const url = activeUserId ? `${API_BASE_URL}?userId=${encodeURIComponent(activeUserId)}` : API_BASE_URL
    const response = await fetch(url)
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch sessions (HTTP ${response.status})`)
    }
    return await response.json()
  },

  async createSession(sessionPayload) {
    const activeUserId = getStoredUserId(sessionPayload?.userId)
    const payload = {
      ...sessionPayload,
      userId: activeUserId || sessionPayload?.userId
    }

    const response = await fetch(API_BASE_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to create session (HTTP ${response.status})`)
    }

    return await response.json()
  },

  async getGoalSets(goalRequestDto) {
    const activeUserId = getStoredUserId(goalRequestDto?.userId)
    const payload = {
      ...goalRequestDto,
      userId: activeUserId || goalRequestDto?.userId
    }

    const response = await fetch(`${API_BASE_URL}/goal-sets`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to fetch goal sets (HTTP ${response.status})`)
    }

    return await response.json()
  },

  async getTemplates(userId = null) {
    const activeUserId = getStoredUserId(userId)
    const url = activeUserId ? `${API_BASE_URL}/templates?userId=${encodeURIComponent(activeUserId)}` : `${API_BASE_URL}/templates`
    try {
      const response = await fetch(url)
      if (response.ok) {
        return await response.json()
      }
    } catch {
      // Endpoint may not be active yet on backend; return empty array or handle error
    }
    return []
  },

  async createTemplate(templatePayload) {
    const activeUserId = getStoredUserId(templatePayload?.userId)
    const payload = {
      ...templatePayload,
      userId: activeUserId || templatePayload?.userId
    }

    const response = await fetch(`${API_BASE_URL}/templates`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to create exercise template (HTTP ${response.status})`)
    }

    return await response.json()
  },

  async getWorkoutTemplates(userId = null) {
    const activeUserId = getStoredUserId(userId)
    const url = activeUserId ? `${API_BASE_URL}/workout-templates?userId=${encodeURIComponent(activeUserId)}` : `${API_BASE_URL}/workout-templates`
    try {
      const response = await fetch(url)
      if (response.ok) {
        return await response.json()
      }
    } catch {
      // Endpoint fallback
    }
    return []
  },

  async createWorkoutTemplate(workoutTemplatePayload) {
    const activeUserId = getStoredUserId(workoutTemplatePayload?.userId)
    const payload = {
      ...workoutTemplatePayload,
      userId: activeUserId || workoutTemplatePayload?.userId
    }

    const response = await fetch(`${API_BASE_URL}/workout-templates`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || `Failed to create workout template (HTTP ${response.status})`)
    }

    return await response.json()
  }
}

export default WorkoutSessionApi

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5245/api/WorkoutSession'

const getStoredUserId = (userId = null) => {
  if (userId) return userId
  if (typeof localStorage !== 'undefined') {
    return localStorage.getItem('userId') || localStorage.getItem('overload_user_id') || null
  }
  return null
}

/**
 * Service layer for Template View
 * Provides CRUD operations for workout templates and exercise templates
 */
const TemplateApi = {
  async getWorkoutTemplates(userId = null) {
    const activeUserId = getStoredUserId(userId)
    const url = activeUserId ? `${API_BASE_URL}/workout-templates?userId=${encodeURIComponent(activeUserId)}` : `${API_BASE_URL}/workout-templates`
    try {
      const response = await fetch(url)
      if (response.ok) {
        return await response.json()
      }
    } catch {
      // Graceful fallback for local dev
    }
    return []
  },

  async createWorkoutTemplate(workoutTemplatePayload) {
    const activeUserId = getStoredUserId(workoutTemplatePayload?.userId)
    const payload = {
      ...workoutTemplatePayload,
      userId: activeUserId || workoutTemplatePayload?.userId
    }

    try {
      const response = await fetch(`${API_BASE_URL}/workout-templates`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      })

      if (response.ok) {
        return await response.json()
      }
    } catch {
      // If endpoint is not yet present on backend, return local object with id
    }

    return {
      id: Date.now(),
      ...payload
    }
  },

  async updateWorkoutTemplate(id, workoutTemplatePayload) {
    const activeUserId = getStoredUserId(workoutTemplatePayload?.userId)
    const payload = {
      ...workoutTemplatePayload,
      id,
      userId: activeUserId || workoutTemplatePayload?.userId
    }

    try {
      const response = await fetch(`${API_BASE_URL}/workout-templates/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      })

      if (response.ok) {
        return await response.json()
      }
    } catch {
      // Local fallback
    }

    return payload
  },

  async deleteWorkoutTemplate(id) {
    try {
      const response = await fetch(`${API_BASE_URL}/workout-templates/${id}`, {
        method: 'DELETE'
      })

      if (response.ok) {
        return { success: true, id }
      }
    } catch {
      // Local fallback
    }

    return { success: true, id }
  },

  async getExerciseTemplates(userId = null) {
    const activeUserId = getStoredUserId(userId)
    const url = activeUserId ? `${API_BASE_URL}/templates?userId=${encodeURIComponent(activeUserId)}` : `${API_BASE_URL}/templates`
    try {
      const response = await fetch(url)
      if (response.ok) {
        return await response.json()
      }
    } catch {
      // Local fallback
    }
    return []
  },

  async createExerciseTemplate(templatePayload) {
    const activeUserId = getStoredUserId(templatePayload?.userId)
    const payload = {
      ...templatePayload,
      userId: activeUserId || templatePayload?.userId
    }

    try {
      const response = await fetch(`${API_BASE_URL}/templates`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      })

      if (response.ok) {
        return await response.json()
      }
    } catch {
      // Fallback
    }

    return {
      id: Date.now(),
      ...payload
    }
  }
}

export default TemplateApi

import WorkoutSessionApi from './WorkoutSession.api.js'

/**
 * Service layer for Session View
 * Enforces: Service Layer -> Parent Component -> Child Component flow
 */
const SessionApi = {
  async getUsers() {
    return await WorkoutSessionApi.getUsers()
  },

  async getSessions(userId = null) {
    return await WorkoutSessionApi.getSessions(userId)
  },

  async createSession(sessionPayload) {
    return await WorkoutSessionApi.createSession(sessionPayload)
  },

  async getGoalSets(goalRequestDto) {
    return await WorkoutSessionApi.getGoalSets(goalRequestDto)
  },

  async getWorkoutTemplates(userId = null) {
    return await WorkoutSessionApi.getWorkoutTemplates(userId)
  },

  async getExerciseTemplates(userId = null) {
    return await WorkoutSessionApi.getTemplates(userId)
  }
}

export default SessionApi

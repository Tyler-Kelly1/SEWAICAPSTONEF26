import { describe, it, expect, vi, beforeEach } from 'vitest'
import SessionApi from './Session.api.js'
import WorkoutSessionApi from './WorkoutSession.api.js'

describe('SessionApi Service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('delegates getUsers to WorkoutSessionApi', async () => {
    const mockUsers = [{ userId: 'tyler_dev' }]
    vi.spyOn(WorkoutSessionApi, 'getUsers').mockResolvedValue(mockUsers)

    const result = await SessionApi.getUsers()
    expect(WorkoutSessionApi.getUsers).toHaveBeenCalled()
    expect(result).toEqual(mockUsers)
  })

  it('delegates createSession to WorkoutSessionApi', async () => {
    const payload = { userId: 'tyler_dev', workoutName: 'Bench Test' }
    vi.spyOn(WorkoutSessionApi, 'createSession').mockResolvedValue({ id: 1, ...payload })

    const result = await SessionApi.createSession(payload)
    expect(WorkoutSessionApi.createSession).toHaveBeenCalledWith(payload)
    expect(result.id).toBe(1)
  })

  it('delegates getGoalSets to WorkoutSessionApi', async () => {
    const goalReq = { userId: 'tyler_dev', exerciseName: 'Squat', sets: [] }
    vi.spyOn(WorkoutSessionApi, 'getGoalSets').mockResolvedValue({ exerciseName: 'Squat', goalSets: [] })

    const result = await SessionApi.getGoalSets(goalReq)
    expect(WorkoutSessionApi.getGoalSets).toHaveBeenCalledWith(goalReq)
    expect(result.exerciseName).toBe('Squat')
  })
})

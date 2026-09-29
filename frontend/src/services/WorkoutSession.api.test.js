import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import WorkoutSessionApi from './WorkoutSession.api.js'

describe('WorkoutSessionApi Service', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('getUsers returns parsed JSON array on success', async () => {
    const mockUsers = [
      { userId: 'tyler_dev', sessions: [] },
      { userId: 'alex_fit', sessions: [] }
    ]

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockUsers
    })

    const result = await WorkoutSessionApi.getUsers()
    expect(fetch).toHaveBeenCalledTimes(1)
    expect(result).toEqual(mockUsers)
  })

  it('getSessions fetches all sessions when no userId is passed', async () => {
    const mockSessions = [
      { id: 1, userId: 'tyler_dev', executedWorkout: { workoutName: 'Upper Body' } }
    ]

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockSessions
    })

    const result = await WorkoutSessionApi.getSessions()
    expect(fetch).toHaveBeenCalledTimes(1)
    expect(result).toEqual(mockSessions)
  })

  it('getSessions appends userId query parameter when provided', async () => {
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => []
    })

    await WorkoutSessionApi.getSessions('alex_fit')
    expect(fetch).toHaveBeenCalledWith(expect.stringContaining('userId=alex_fit'))
  })

  it('createSession sends POST request with JSON payload', async () => {
    const payload = {
      userId: 'tyler_dev',
      workoutName: 'Morning Lift',
      startTime: '2026-09-15T08:00:00Z',
      endTime: '2026-09-15T09:00:00Z',
      exercises: [
        {
          exerciseName: 'Bench Press',
          sets: [{ weight: 185, reps: 8 }]
        }
      ]
    }

    const mockResponse = { id: 10, ...payload }

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockResponse
    })

    const result = await WorkoutSessionApi.createSession(payload)
    expect(fetch).toHaveBeenCalledWith(
      expect.any(String),
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
    )
    expect(result).toEqual(mockResponse)
  })

  it('getGoalSets sends POST request to /goal-sets', async () => {
    const goalRequest = {
      userId: 'tyler_dev',
      exerciseName: 'Barbell Bench Press',
      sets: []
    }

    const mockGoalResponse = {
      exerciseName: 'Barbell Bench Press',
      goalSets: [{ setNumber: 1, weight: 195, reps: 6 }]
    }

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockGoalResponse
    })

    const result = await WorkoutSessionApi.getGoalSets(goalRequest)
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/goal-sets'),
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(goalRequest)
      })
    )
    expect(result).toEqual(mockGoalResponse)
  })

  it('createTemplate sends POST request to /templates', async () => {
    const templatePayload = {
      userId: 'tyler_dev',
      exercise_Name: 'Incline Bench Press',
      min_Set: 3,
      max_Set: 5,
      weight_Step: 0.05,
      volume_Step: 1,
      setTemplates: [{ min_Reps: 8, max_Reps: 12, failure_Set: false }]
    }

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ id: 5, ...templatePayload })
    })

    const result = await WorkoutSessionApi.createTemplate(templatePayload)
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/templates'),
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
      })
    )
    expect(result.id).toBe(5)
  })

  it('createWorkoutTemplate sends POST request to /workout-templates', async () => {
    const workoutTemplatePayload = {
      userId: 'tyler_dev',
      workout_Name: 'Leg Day Blitz',
      exerciseTemplates: []
    }

    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ id: 2, ...workoutTemplatePayload })
    })

    const result = await WorkoutSessionApi.createWorkoutTemplate(workoutTemplatePayload)
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/workout-templates'),
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
      })
    )
    expect(result.id).toBe(2)
  })
})

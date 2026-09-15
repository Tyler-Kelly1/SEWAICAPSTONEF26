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
})

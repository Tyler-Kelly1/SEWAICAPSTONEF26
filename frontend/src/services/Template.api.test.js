import { describe, it, expect, vi, beforeEach } from 'vitest'
import TemplateApi from './Template.api.js'

describe('TemplateApi Service', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
  })

  it('getWorkoutTemplates fetches list of workout templates', async () => {
    const mockTemplates = [
      { id: 1, workout_Name: 'Push Day', exerciseTemplates: [] }
    ]
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockTemplates
    })

    const result = await TemplateApi.getWorkoutTemplates('tyler_dev')
    expect(result).toEqual(mockTemplates)
  })

  it('createWorkoutTemplate sends POST and returns created template', async () => {
    const newTemplate = {
      userId: 'tyler_dev',
      workout_Name: 'Pull Day',
      exerciseTemplates: []
    }
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ id: 5, ...newTemplate })
    })

    const result = await TemplateApi.createWorkoutTemplate(newTemplate)
    expect(result.id).toBe(5)
    expect(result.workout_Name).toBe('Pull Day')
  })

  it('updateWorkoutTemplate sends PUT and returns updated template', async () => {
    const updated = {
      id: 2,
      userId: 'tyler_dev',
      workout_Name: 'Leg Day Updated',
      exerciseTemplates: []
    }
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => updated
    })

    const result = await TemplateApi.updateWorkoutTemplate(2, updated)
    expect(result.workout_Name).toBe('Leg Day Updated')
  })

  it('deleteWorkoutTemplate sends DELETE request', async () => {
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({})
    })

    const result = await TemplateApi.deleteWorkoutTemplate(2)
    expect(result.success).toBe(true)
    expect(result.id).toBe(2)
  })
})

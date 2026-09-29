import { describe, it, expect, vi, beforeEach } from 'vitest'
import UserApi from './User.api.js'

describe('UserApi', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
  })

  it('checkUserExists sends GET request to exists endpoint', async () => {
    const mockResult = { userId: 'tyler_dev', exists: true }
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => mockResult
    })

    const result = await UserApi.checkUserExists('tyler_dev')
    expect(global.fetch).toHaveBeenCalledWith('http://localhost:5245/api/User/exists/tyler_dev')
    expect(result).toEqual(mockResult)
  })

  it('checkUserExists returns exists false on empty input without fetch', async () => {
    global.fetch = vi.fn()
    const result = await UserApi.checkUserExists('   ')
    expect(global.fetch).not.toHaveBeenCalled()
    expect(result).toEqual({ exists: false, userId: '' })
  })

  it('getUser sends GET request to userId endpoint', async () => {
    const mockUser = { userId: 'athlete_1' }
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => mockUser
    })

    const result = await UserApi.getUser('athlete_1')
    expect(global.fetch).toHaveBeenCalledWith('http://localhost:5245/api/User/athlete_1')
    expect(result).toEqual(mockUser)
  })

  it('getUser throws error if response is not ok', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 404,
      text: async () => 'User not found'
    })

    await expect(UserApi.getUser('nonexistent')).rejects.toThrow('User not found')
  })

  it('createUser sends POST request with userId', async () => {
    const mockCreated = { userId: 'new_lifter' }
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => mockCreated
    })

    const result = await UserApi.createUser({ userId: 'new_lifter' })
    expect(global.fetch).toHaveBeenCalledWith('http://localhost:5245/api/User', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ userId: 'new_lifter' })
    })
    expect(result).toEqual(mockCreated)
  })

  it('createUser throws error when API returns error', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 409,
      text: async () => 'User already exists'
    })

    await expect(UserApi.createUser({ userId: 'already_exists' })).rejects.toThrow('User already exists')
  })

  it('getUsers sends GET request to root User endpoint', async () => {
    const mockUsers = [{ userId: 'user1' }, { userId: 'user2' }]
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => mockUsers
    })

    const result = await UserApi.getUsers()
    expect(global.fetch).toHaveBeenCalledWith('http://localhost:5245/api/User')
    expect(result).toEqual(mockUsers)
  })
})

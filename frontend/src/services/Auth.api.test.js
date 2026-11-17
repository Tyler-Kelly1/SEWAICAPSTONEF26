import { describe, it, expect, beforeEach, vi } from 'vitest'
import AuthApi from './Auth.api.js'

describe('Auth.api.js', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.restoreAllMocks()
  })

  it('records user activity and reports session not expired within 24 hours', () => {
    AuthApi.recordActivity()
    expect(AuthApi.getLastActivity()).toBeTypeOf('number')
    expect(AuthApi.isSessionExpired()).toBe(false)
  })

  it('detects session expired when inactivity exceeds 24 hours', () => {
    const twentyFiveHoursAgo = Date.now() - (25 * 60 * 60 * 1000)
    localStorage.setItem('overload_last_activity', twentyFiveHoursAgo.toString())

    expect(AuthApi.isSessionExpired()).toBe(true)
  })

  it('clears session removes all auth-related keys from localStorage', () => {
    localStorage.setItem('userId', 'tyler_dev')
    localStorage.setItem('overload_user_id', 'tyler_dev')
    localStorage.setItem('overload_last_activity', Date.now().toString())

    AuthApi.clearSession()

    expect(localStorage.getItem('userId')).toBeNull()
    expect(localStorage.getItem('overload_user_id')).toBeNull()
    expect(localStorage.getItem('overload_last_activity')).toBeNull()
  })

  it('login sends credentials: include and persists session on success', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({
        success: true,
        userId: 'tyler_dev',
        userName: 'tyler_dev',
        email: 'tyler@example.com'
      })
    })

    const result = await AuthApi.login({ email: 'tyler@example.com', password: 'Password123!' })

    expect(result.success).toBe(true)
    expect(global.fetch).toHaveBeenCalledWith(
      expect.stringContaining('/login'),
      expect.objectContaining({
        method: 'POST',
        credentials: 'include'
      })
    )
    expect(localStorage.getItem('userId')).toBe('tyler_dev')
    expect(AuthApi.getLastActivity()).toBeTypeOf('number')
  })

  it('register sends credentials: include and calls backend API', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({
        success: true,
        message: 'Account registered successfully.'
      })
    })

    const result = await AuthApi.register({
      username: 'new_athlete',
      email: 'new@example.com',
      password: 'Password123!'
    })

    expect(result.success).toBe(true)
    expect(global.fetch).toHaveBeenCalledWith(
      expect.stringContaining('/register'),
      expect.objectContaining({
        method: 'POST',
        credentials: 'include'
      })
    )
  })
})

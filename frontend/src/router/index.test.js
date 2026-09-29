import { describe, it, expect, beforeEach } from 'vitest'
import { useHashRouter } from './index.js'

describe('useHashRouter', () => {
  beforeEach(() => {
    window.location.hash = ''
  })

  it('defaults currentRoute to / when hash is empty or #/', () => {
    window.location.hash = ''
    const router = useHashRouter()
    expect(router.currentRoute.value).toBe('/')

    window.location.hash = '#/'
    expect(router.currentRoute.value).toBe('/')
  })

  it('normalizes currentRoute correctly for hash paths', () => {
    window.location.hash = '#/login'
    window.dispatchEvent(new Event('hashchange'))
    const router = useHashRouter()
    expect(router.currentRoute.value).toBe('/login')
  })

  it('navigateTo changes window.location.hash', () => {
    const router = useHashRouter()
    router.navigateTo('/login')
    expect(window.location.hash).toBe('#/login')

    router.navigateTo('dashboard')
    expect(window.location.hash).toBe('#/dashboard')
  })
})

import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import LoginView from './LoginView.vue'
import AuthApi from '../services/Auth.api.js'

describe('LoginView.vue', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    localStorage.clear()
    window.location.hash = '#/login'
  })

  const defaultStubs = {
    'v-alert': { template: '<div><slot /></div>' },
    WelcomeLogin: {
      template: `
        <div>
          <button data-testid="stub-login" @click="$emit('login', { email: 'tyler@example.com', password: 'Password123!', callback: () => {} })">Login</button>
          <button data-testid="stub-login-success" @click="$emit('login-success', 'tyler_dev')">Login Success</button>
        </div>
      `
    }
  }

  it('renders LoginView properly', () => {
    const wrapper = mount(LoginView, {
      global: { stubs: defaultStubs }
    })

    expect(wrapper.find('[data-testid="login-view"]').exists()).toBe(true)
  })

  it('handles login calls via AuthApi and triggers callback', async () => {
    vi.spyOn(AuthApi, 'login').mockResolvedValue({ success: true, userId: 'tyler_dev' })

    const wrapper = mount(LoginView, {
      global: { stubs: defaultStubs }
    })

    const callback = vi.fn()
    await wrapper.vm.handleLogin({
      email: 'tyler@example.com',
      password: 'Password123!',
      callback
    })

    expect(AuthApi.login).toHaveBeenCalledWith({ email: 'tyler@example.com', password: 'Password123!' })
    expect(callback).toHaveBeenCalledWith(expect.objectContaining({ success: true }))
  })

  it('handles register calls via AuthApi and auto-logs in', async () => {
    vi.spyOn(AuthApi, 'register').mockResolvedValue({ success: true })
    vi.spyOn(AuthApi, 'login').mockResolvedValue({ success: true, userId: 'new_athlete' })

    const wrapper = mount(LoginView, {
      global: { stubs: defaultStubs }
    })

    const callback = vi.fn()
    await wrapper.vm.handleRegister({
      username: 'new_athlete',
      email: 'new@example.com',
      password: 'Password123!',
      callback
    })

    expect(AuthApi.register).toHaveBeenCalledWith({ username: 'new_athlete', email: 'new@example.com', password: 'Password123!' })
    expect(AuthApi.login).toHaveBeenCalledWith({ email: 'new@example.com', password: 'Password123!' })
    expect(callback).toHaveBeenCalledWith(expect.objectContaining({ success: true }))
  })

  it('redirects to #/session on login success', () => {
    const wrapper = mount(LoginView, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.handleLoginSuccess('athlete_99')

    expect(wrapper.emitted('login-success')).toBeTruthy()
    expect(wrapper.emitted('login-success')[0][0]).toBe('athlete_99')
    expect(window.location.hash).toBe('#/session')
  })
})

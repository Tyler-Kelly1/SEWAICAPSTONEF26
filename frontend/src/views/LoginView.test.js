import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import LoginView from './LoginView.vue'
import UserApi from '../services/User.api.js'

describe('LoginView.vue', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    window.location.hash = '#/login'
  })

  const defaultStubs = {
    'v-alert': { template: '<div><slot /></div>' },
    WelcomeLogin: {
      template: `
        <div>
          <button data-testid="stub-check" @click="$emit('check-user', { userId: 'tyler_dev', callback: () => {} })">Check</button>
          <button data-testid="stub-login" @click="$emit('login-success', 'tyler_dev')">Login</button>
        </div>
      `
    }
  }

  it('renders LoginView and loads existing users on mount', async () => {
    vi.spyOn(UserApi, 'getUsers').mockResolvedValue([{ userId: 'tyler_dev' }])

    const wrapper = mount(LoginView, {
      global: { stubs: defaultStubs }
    })

    expect(wrapper.find('[data-testid="login-view"]').exists()).toBe(true)
    await wrapper.vm.$nextTick()
    expect(UserApi.getUsers).toHaveBeenCalled()
  })

  it('stores user in localStorage and redirects to #/ on login success', async () => {
    vi.spyOn(UserApi, 'getUsers').mockResolvedValue([])

    const wrapper = mount(LoginView, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.handleLoginSuccess('athlete_99')

    expect(localStorage.getItem('userId')).toBe('athlete_99')
    expect(wrapper.emitted('login-success')).toBeTruthy()
    expect(wrapper.emitted('login-success')[0][0]).toBe('athlete_99')
    expect(window.location.hash).toBe('#/')
  })
})

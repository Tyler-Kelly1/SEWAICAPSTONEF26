import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import WelcomeLogin from './WelcomeLogin.vue'

describe('WelcomeLogin.vue', () => {
  const defaultStubs = {
    'v-card': { template: '<div><slot /></div>' },
    'v-icon': true,
    'v-alert': { template: '<div><slot /></div>' }
  }

  it('renders welcome message and login form by default', () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    expect(wrapper.text()).toContain('Welcome to Overload')
    expect(wrapper.text()).toContain('ATHLETE EMAIL ADDRESS')
    expect(wrapper.find('[data-testid="input-login-email"]').exists()).toBe(true)
    expect(wrapper.find('[data-testid="input-login-password"]').exists()).toBe(true)
  })

  it('switches between Login and Register tabs', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    const registerTab = wrapper.find('[data-testid="tab-register-mode"]')
    await registerTab.trigger('click')

    expect(wrapper.vm.activeMode).toBe('register')
    expect(wrapper.find('[data-testid="input-register-username"]').exists()).toBe(true)
    expect(wrapper.find('[data-testid="input-register-email"]').exists()).toBe(true)
    expect(wrapper.find('[data-testid="input-register-password"]').exists()).toBe(true)
  })

  it('emits login event on valid login submission', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.loginEmail = 'tyler@example.com'
    wrapper.vm.loginPassword = 'Password123!'
    wrapper.vm.handleLoginSubmit()

    expect(wrapper.emitted('login')).toBeTruthy()
    const loginCall = wrapper.emitted('login')[0][0]
    expect(loginCall.email).toBe('tyler@example.com')
    expect(loginCall.password).toBe('Password123!')
    expect(typeof loginCall.callback).toBe('function')
  })

  it('emits login-success when login callback succeeds', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.loginEmail = 'tyler@example.com'
    wrapper.vm.loginPassword = 'Password123!'
    wrapper.vm.handleLoginSubmit()

    const loginCall = wrapper.emitted('login')[0][0]
    loginCall.callback({ success: true, user: { userId: 'tyler_dev' } })

    expect(wrapper.emitted('login-success')).toBeTruthy()
    expect(wrapper.emitted('login-success')[0][0]).toBe('tyler_dev')
  })

  it('emits register event on valid account creation submission', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.switchMode('register')
    wrapper.vm.registerUsername = 'new_lifter'
    wrapper.vm.registerEmail = 'new@example.com'
    wrapper.vm.registerPassword = 'Password123!'
    wrapper.vm.registerConfirmPassword = 'Password123!'

    wrapper.vm.handleRegisterSubmit()

    expect(wrapper.emitted('register')).toBeTruthy()
    const regCall = wrapper.emitted('register')[0][0]
    expect(regCall.username).toBe('new_lifter')
    expect(regCall.email).toBe('new@example.com')
    expect(regCall.password).toBe('Password123!')

    regCall.callback({ success: true, user: { userId: 'new_lifter' } })
    expect(wrapper.emitted('login-success')).toBeTruthy()
    expect(wrapper.emitted('login-success')[0][0]).toBe('new_lifter')
  })
})

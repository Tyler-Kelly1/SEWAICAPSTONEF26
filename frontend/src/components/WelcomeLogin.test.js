import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import WelcomeLogin from './WelcomeLogin.vue'

describe('WelcomeLogin.vue', () => {
  const defaultStubs = {
    'v-card': { template: '<div><slot /></div>' },
    'v-card-text': { template: '<div><slot /></div>' },
    'v-avatar': { template: '<div><slot /></div>' },
    'v-icon': true,
    'v-divider': true,
    'v-alert': { template: '<div><slot /></div>' },
    'v-fade-transition': { template: '<div><slot /></div>' },
    'v-form': { template: '<div><slot /></div>' },
    'v-text-field': true,
    'v-btn': { template: '<button><slot /></button>' },
    'v-chip': { template: '<span><slot /></span>' }
  }

  it('renders welcome message and input field', () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs },
      props: { existingUsers: [{ userId: 'tyler_dev' }] }
    })

    expect(wrapper.text()).toContain('Welcome to Overload')
    expect(wrapper.text()).toContain('Enter your User ID')
  })

  it('emits check-user event on continue', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.inputUserId = 'tyler_dev'
    wrapper.vm.handleContinue()

    expect(wrapper.emitted('check-user')).toBeTruthy()
    const checkCall = wrapper.emitted('check-user')[0][0]
    expect(checkCall.userId).toBe('tyler_dev')
    expect(typeof checkCall.callback).toBe('function')
  })

  it('emits login-success if check callback indicates user exists', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.inputUserId = 'tyler_dev'
    wrapper.vm.handleContinue()

    const checkCall = wrapper.emitted('check-user')[0][0]
    checkCall.callback({ exists: true, userId: 'tyler_dev' })

    expect(wrapper.emitted('login-success')).toBeTruthy()
    expect(wrapper.emitted('login-success')[0][0]).toBe('tyler_dev')
  })

  it('sets userNotFound to true if check callback indicates user does not exist', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.inputUserId = 'brand_new_user'
    wrapper.vm.handleContinue()

    const checkCall = wrapper.emitted('check-user')[0][0]
    checkCall.callback({ exists: false, userId: 'brand_new_user' })

    expect(wrapper.vm.userNotFound).toBe(true)
    expect(wrapper.vm.pendingUserId).toBe('brand_new_user')
  })

  it('emits create-user event when creating account', async () => {
    const wrapper = mount(WelcomeLogin, {
      global: { stubs: defaultStubs }
    })

    wrapper.vm.pendingUserId = 'brand_new_user'
    wrapper.vm.handleCreateAccount()

    expect(wrapper.emitted('create-user')).toBeTruthy()
    const createCall = wrapper.emitted('create-user')[0][0]
    expect(createCall.userId).toBe('brand_new_user')

    createCall.callback({ success: true, userId: 'brand_new_user' })
    expect(wrapper.emitted('login-success')).toBeTruthy()
    expect(wrapper.emitted('login-success')[0][0]).toBe('brand_new_user')
  })
})

import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ActiveWorkoutSession from './ActiveWorkoutSession.vue'

describe('ActiveWorkoutSession.vue', () => {
  it('renders pre-session form and emits fetch-goal-sets on start', async () => {
    const wrapper = mount(ActiveWorkoutSession, {
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-chip': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' }
        }
      },
      props: {
        users: [{ userId: 'tyler_dev' }],
        templates: [],
        submitting: false
      }
    })

    expect(wrapper.text()).toContain('Start Exercise Session')
    
    wrapper.vm.startSession()

    expect(wrapper.emitted('fetch-goal-sets')).toBeTruthy()
  })
})

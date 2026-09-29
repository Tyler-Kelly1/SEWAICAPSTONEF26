import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import SessionView from './SessionView.vue'
import SessionApi from '../services/Session.api.js'
import UserApi from '../services/User.api.js'

describe('SessionView.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(UserApi, 'getUsers').mockResolvedValue([{ userId: 'tyler_dev' }])
    vi.spyOn(SessionApi, 'getWorkoutTemplates').mockResolvedValue([
      {
        id: 1,
        userId: 'tyler_dev',
        workout_Name: 'Power Upper',
        exerciseTemplates: [{ id: 1, exercise_Name: 'Bench Press' }]
      }
    ])
    vi.spyOn(SessionApi, 'getExerciseTemplates').mockResolvedValue([
      { id: 1, exercise_Name: 'Bench Press' }
    ])
    vi.spyOn(SessionApi, 'createSession').mockResolvedValue({ id: 101, workoutName: 'Power Upper' })
  })

  it('renders session view and mounts ActiveWorkoutSession', () => {
    const wrapper = mount(SessionView, {
      props: { currentUserId: 'tyler_dev' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-btn': true,
          'v-alert': { template: '<div><slot /></div>' },
          ActiveWorkoutSession: {
            name: 'ActiveWorkoutSession',
            template: '<div data-testid="active-session-stub">Active Session Stub</div>'
          }
        }
      }
    })

    expect(wrapper.find('[data-testid="session-view"]').exists()).toBe(true)
    expect(wrapper.text()).toContain('Active Session View')
    expect(wrapper.text()).toContain('Any active session MUST follow a template')
  })

  it('handles save-session event and invokes SessionApi', async () => {
    const wrapper = mount(SessionView, {
      props: { currentUserId: 'tyler_dev' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-btn': true,
          'v-alert': { template: '<div><slot /></div>' },
          ActiveWorkoutSession: true
        }
      }
    })

    await wrapper.vm.handleSaveSession({
      workoutName: 'Power Upper',
      exercises: []
    })

    expect(SessionApi.createSession).toHaveBeenCalledWith(
      expect.objectContaining({
        workoutName: 'Power Upper',
        userId: 'tyler_dev'
      })
    )
    expect(wrapper.emitted('session-saved')).toBeTruthy()
  })
})

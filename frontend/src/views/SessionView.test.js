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

  it('loads and resolves all available workout templates when fetched from SessionApi', async () => {
    vi.spyOn(SessionApi, 'getWorkoutTemplates').mockResolvedValue([
      { id: 1, workout_Name: 'Upper Body Power Template', exerciseTemplates: [] },
      { id: 2, workout_Name: 'Lower Body Power Template', exerciseTemplates: [] },
      { id: 3, workout_Name: 'Push Day Hypertrophy', exerciseTemplates: [] }
    ])

    const wrapper = mount(SessionView, {
      props: { currentUserId: 'tyler_dev' },
      global: {
        stubs: {
          'v-card': true,
          'v-card-item': true,
          'v-card-title': true,
          'v-card-subtitle': true,
          'v-icon': true,
          'v-btn': true,
          'v-alert': true,
          ActiveWorkoutSession: {
            name: 'ActiveWorkoutSession',
            props: ['workoutTemplates'],
            template: '<div data-testid="active-session-stub">{{ workoutTemplates.map(t => t.workout_Name).join(", ") }}</div>'
          }
        }
      }
    })

    await wrapper.vm.$nextTick()
    await new Promise(resolve => setTimeout(resolve, 10))

    const stub = wrapper.find('[data-testid="active-session-stub"]')
    expect(stub.text()).toContain('Upper Body Power Template')
    expect(stub.text()).toContain('Lower Body Power Template')
    expect(stub.text()).toContain('Push Day Hypertrophy')
  })

  it('reacts to props.workoutTemplates updates providing all passed templates', async () => {
    const wrapper = mount(SessionView, {
      props: {
        currentUserId: 'tyler_dev',
        workoutTemplates: [
          { id: 1, workout_Name: 'Upper Body Power Template', exerciseTemplates: [] },
          { id: 2, workout_Name: 'Lower Body Power Template', exerciseTemplates: [] }
        ]
      },
      global: {
        stubs: {
          'v-card': true,
          'v-card-item': true,
          'v-card-title': true,
          'v-card-subtitle': true,
          'v-icon': true,
          'v-btn': true,
          'v-alert': true,
          ActiveWorkoutSession: {
            name: 'ActiveWorkoutSession',
            props: ['workoutTemplates'],
            template: '<div data-testid="active-session-stub">{{ workoutTemplates.length }} templates</div>'
          }
        }
      }
    })

    await wrapper.vm.$nextTick()
    expect(wrapper.find('[data-testid="active-session-stub"]').text()).toBe('2 templates')

    await wrapper.setProps({
      workoutTemplates: [
        { id: 1, workout_Name: 'Upper Body Power Template', exerciseTemplates: [] },
        { id: 2, workout_Name: 'Lower Body Power Template', exerciseTemplates: [] },
        { id: 3, workout_Name: 'Push Day Hypertrophy', exerciseTemplates: [] }
      ]
    })
    await wrapper.vm.$nextTick()
    expect(wrapper.find('[data-testid="active-session-stub"]').text()).toBe('3 templates')
  })
})

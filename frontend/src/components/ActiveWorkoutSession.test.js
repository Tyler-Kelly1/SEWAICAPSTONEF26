import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ActiveWorkoutSession from './ActiveWorkoutSession.vue'

describe('ActiveWorkoutSession.vue', () => {
  const commonStubs = {
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
    'v-form': { template: '<div><slot /></div>' },
    'v-table': { template: '<table><slot /></table>' }
  }

  const sampleWorkoutTemplates = [
    {
      id: 1,
      workout_Name: 'Upper Body Power Template',
      exerciseTemplates: [
        {
          id: 1,
          exercise_Name: 'Barbell Bench Press',
          min_Set: 3,
          setTemplates: [{ id: 1 }, { id: 2 }, { id: 3 }]
        }
      ]
    },
    {
      id: 2,
      workout_Name: 'Lower Body Power Template',
      exerciseTemplates: [
        {
          id: 3,
          exercise_Name: 'Barbell Back Squat',
          min_Set: 3,
          setTemplates: [{ id: 4 }, { id: 5 }]
        }
      ]
    }
  ]

  it('renders pre-session form and emits fetch-goal-sets on start', async () => {
    const wrapper = mount(ActiveWorkoutSession, {
      global: { stubs: commonStubs },
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

  it('initializes cleanly when mounted with workoutTemplates without temporal dead zone error', () => {
    expect(() => {
      mount(ActiveWorkoutSession, {
        global: { stubs: commonStubs },
        props: {
          users: [{ userId: 'tyler_dev' }],
          workoutTemplates: sampleWorkoutTemplates,
          templates: [],
          submitting: false
        }
      })
    }).not.toThrow()
  })

  it('applies selected workout template and populates corresponding exercises', async () => {
    const wrapper = mount(ActiveWorkoutSession, {
      global: { stubs: commonStubs },
      props: {
        users: [{ userId: 'tyler_dev' }],
        workoutTemplates: sampleWorkoutTemplates,
        templates: [],
        submitting: false
      }
    })

    expect(wrapper.vm.workoutName).toBe('Upper Body Power Template')
    expect(wrapper.vm.exercises.length).toBe(1)
    expect(wrapper.vm.exercises[0].name).toBe('Barbell Bench Press')

    wrapper.vm.applyWorkoutTemplate(2)
    expect(wrapper.vm.workoutName).toBe('Lower Body Power Template')
    expect(wrapper.vm.exercises[0].name).toBe('Barbell Back Squat')
    expect(wrapper.vm.exercises[0].sets.length).toBe(2)
  })
})

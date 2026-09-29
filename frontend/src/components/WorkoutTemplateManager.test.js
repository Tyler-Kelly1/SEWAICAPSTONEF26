import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import WorkoutTemplateManager from './WorkoutTemplateManager.vue'

describe('WorkoutTemplateManager.vue', () => {
  it('renders existing workout templates and handles form submission', async () => {
    const mockWorkoutTemplates = [
      {
        id: 1,
        userId: 'tyler_dev',
        workout_Name: 'Upper Body Power Template',
        exerciseTemplates: [
          { id: 1, exercise_Name: 'Barbell Bench Press', min_Set: 3, max_Set: 5 }
        ]
      }
    ]

    const wrapper = mount(WorkoutTemplateManager, {
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
          'v-chip': { template: '<span><slot /></span>' },
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' },
          'v-list': { template: '<div><slot /></div>' },
          'v-list-item': { template: '<div><slot /></div>' },
          'v-list-item-title': { template: '<div><slot /></div>' },
          'v-list-item-subtitle': { template: '<div><slot /></div>' }
        }
      },
      props: {
        users: [{ userId: 'tyler_dev' }],
        workoutTemplates: mockWorkoutTemplates,
        exerciseTemplates: [
          { id: 1, userId: 'tyler_dev', exercise_Name: 'Barbell Bench Press' }
        ],
        submitting: false
      }
    })

    expect(wrapper.text()).toContain('Upper Body Power Template')
    expect(wrapper.text()).toContain('Barbell Bench Press')

    wrapper.vm.workoutName = 'Leg Day Power'
    wrapper.vm.selectedExerciseIds = [1]
    wrapper.vm.handleSubmit()

    expect(wrapper.emitted('create-workout-template')).toBeTruthy()
    expect(wrapper.emitted('create-workout-template')[0][0]).toEqual(expect.objectContaining({
      workout_Name: 'Leg Day Power',
      userId: 'tyler_dev'
    }))
  })
})

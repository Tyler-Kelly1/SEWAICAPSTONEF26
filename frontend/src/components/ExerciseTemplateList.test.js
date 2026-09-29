import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ExerciseTemplateList from './ExerciseTemplateList.vue'

describe('ExerciseTemplateList.vue', () => {
  it('renders template exercise names and set prescriptions', () => {
    const mockTemplates = [
      {
        id: 1,
        userId: 'tyler_dev',
        exercise_Name: 'Barbell Bench Press',
        min_Set: 3,
        max_Set: 5,
        weight_Step: 0.05,
        volume_Step: 1,
        setTemplates: [
          { id: 1, min_Reps: 6, max_Reps: 10, failure_Set: false }
        ]
      }
    ]

    const wrapper = mount(ExerciseTemplateList, {
      global: {
        stubs: {
          'v-card': { template: '<div><slot /><slot name="title" /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-btn': true,
          'v-progress-circular': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-chip': { template: '<span><slot /></span>' },
          'v-table': { template: '<table><slot /></table>' }
        }
      },
      props: {
        users: [{ userId: 'tyler_dev' }],
        templates: mockTemplates,
        loading: false
      }
    })

    expect(wrapper.text()).toContain('Barbell Bench Press')
    expect(wrapper.text()).toContain('3 - 5 sets')
  })

  it('renders athlete chip instead of v-select when currentUserId is provided', () => {
    const wrapper = mount(ExerciseTemplateList, {
      global: {
        stubs: {
          'v-card': { template: '<div><slot /><slot name="title" /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': { template: '<div data-testid="user-select"></div>' },
          'v-btn': true,
          'v-progress-circular': true,
          'v-row': true,
          'v-col': true,
          'v-chip': { template: '<span data-testid="chip"><slot /></span>' },
          'v-table': true
        }
      },
      props: {
        currentUserId: 'athlete_sarah',
        users: [{ userId: 'user_1' }, { userId: 'user_2' }],
        templates: [],
        loading: false
      }
    })

    const chip = wrapper.find('[data-testid="exercise-list-user-chip"]')
    expect(chip.exists()).toBe(true)
    expect(chip.text()).toContain('athlete_sarah')
    expect(wrapper.find('[data-testid="user-select"]').exists()).toBe(false)
  })
})

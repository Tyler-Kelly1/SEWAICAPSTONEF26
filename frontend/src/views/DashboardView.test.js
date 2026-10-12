import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import DashboardView from './DashboardView.vue'

describe('DashboardView', () => {
  it('shows the correct workout statistics', () => {
    const sessions = [
      {
        duration: '01:15:00',
        executedWorkout: {
          exercises: [
            { sets: [{}, {}, {}] },
            { sets: [{}, {}] }
          ]
        }
      },
      {
        duration: '00:30:00',
        executedWorkout: {
          exercises: [
            { sets: [{}, {}, {}, {}] },
            { sets: [{}, {}] }
          ]
        }
      }
    ]

    const wrapper = mount(DashboardView, {
      props: { sessions }
    })

    expect(wrapper.get('[data-testid="total-workouts"]').text()).toBe('2')
    expect(wrapper.get('[data-testid="total-workout-time"]').text()).toBe('1h 45m')
    expect(wrapper.get('[data-testid="total-exercises"]').text()).toBe('4')
    expect(wrapper.get('[data-testid="total-sets"]').text()).toBe('11')
  })
})
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import CreateTemplateMenu from './CreateTemplateMenu.vue'

describe('CreateTemplateMenu.vue', () => {
  it('renders form and handles form submission', async () => {
    const wrapper = mount(CreateTemplateMenu, {
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
          'v-checkbox': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' }
        }
      },
      props: {
        users: [{ userId: 'tyler_dev' }],
        submitting: false
      }
    })

    expect(wrapper.text()).toContain('Create Exercise Template Menu')
    
    wrapper.vm.exerciseName = 'Incline Dumbbell Press'
    wrapper.vm.handleSubmit()

    expect(wrapper.emitted('create-template')).toBeTruthy()
    expect(wrapper.emitted('create-template')[0][0]).toEqual(expect.objectContaining({
      exercise_Name: 'Incline Dumbbell Press',
      userId: 'tyler_dev'
    }))
  })

  it('renders target user as a readonly field displaying currentUserId and submits with currentUserId', async () => {
    const wrapper = mount(CreateTemplateMenu, {
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
          'v-text-field': {
            props: ['modelValue', 'readonly', 'disabled'],
            template: '<input :value="modelValue" :disabled="disabled" :readonly="readonly" />'
          },
          'v-btn': true,
          'v-checkbox': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' }
        }
      },
      props: {
        currentUserId: 'athlete_john',
        users: [{ userId: 'other_user' }],
        submitting: false
      }
    })

    const targetUserField = wrapper.find('[data-testid="target-user-field"]')
    expect(targetUserField.exists()).toBe(true)
    expect(targetUserField.attributes('disabled')).toBeDefined()
    expect(targetUserField.attributes('readonly')).toBeDefined()

    wrapper.vm.exerciseName = 'Shoulder Press'
    wrapper.vm.handleSubmit()

    expect(wrapper.emitted('create-template')).toBeTruthy()
    expect(wrapper.emitted('create-template')[0][0]).toEqual(expect.objectContaining({
      exercise_Name: 'Shoulder Press',
      userId: 'athlete_john'
    }))
  })
})

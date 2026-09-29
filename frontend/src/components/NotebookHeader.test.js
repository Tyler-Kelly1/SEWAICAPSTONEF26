import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import NotebookHeader from './NotebookHeader.vue'

describe('NotebookHeader.vue', () => {
  it('renders default title, subtitle, and icon properly', () => {
    const wrapper = mount(NotebookHeader, {
      global: {
        stubs: {
          'v-app-bar': {
            template: '<header class="v-app-bar notebook-app-bar"><slot /></header>'
          },
          'v-icon': {
            template: '<i class="v-icon" :data-icon="icon"><slot /></i>',
            props: ['icon']
          }
        }
      }
    })

    expect(wrapper.find('.header-title').text()).toBe('SEW AI Capstone - Overload')
    expect(wrapper.find('.header-subtitle').text()).toBe('ANALOG GYM JOURNAL // VOL. 1')
    expect(wrapper.find('.binder-tape').exists()).toBe(true)
    expect(wrapper.find('.red-margin-indicator').exists()).toBe(true)
    expect(wrapper.find('[data-testid="current-user-chip"]').exists()).toBe(false)
  })

  it('renders custom title and subtitle passed via props', () => {
    const wrapper = mount(NotebookHeader, {
      props: {
        title: 'Custom Journal',
        subtitle: 'ENTRY #42',
        icon: 'mdi-notebook'
      },
      global: {
        stubs: {
          'v-app-bar': {
            template: '<header class="v-app-bar notebook-app-bar"><slot /></header>'
          },
          'v-icon': {
            template: '<i class="v-icon" :data-icon="icon"></i>',
            props: ['icon']
          }
        }
      }
    })

    expect(wrapper.find('.header-title').text()).toBe('Custom Journal')
    expect(wrapper.find('.header-subtitle').text()).toBe('ENTRY #42')
  })

  it('renders user chip and switch user button when currentUserId is provided', async () => {
    const wrapper = mount(NotebookHeader, {
      props: {
        currentUserId: 'tyler_dev'
      },
      global: {
        stubs: {
          'v-app-bar': {
            template: '<header class="v-app-bar notebook-app-bar"><slot /></header>'
          },
          'v-icon': {
            template: '<i class="v-icon" :data-icon="icon"></i>',
            props: ['icon']
          }
        }
      }
    })

    const userChip = wrapper.find('[data-testid="current-user-chip"]')
    expect(userChip.exists()).toBe(true)
    expect(userChip.text()).toContain('tyler_dev')

    const switchBtn = wrapper.find('[data-testid="btn-switch-user"]')
    expect(switchBtn.exists()).toBe(true)

    await switchBtn.trigger('click')
    expect(wrapper.emitted('switch-user')).toBeTruthy()
    expect(wrapper.emitted('logout')).toBeTruthy()
  })

  it('renders custom slots when provided', () => {
    const wrapper = mount(NotebookHeader, {
      slots: {
        title: '<div class="custom-title-slot">Custom Slot Title</div>',
        append: '<div class="custom-append-slot">Custom Append Content</div>'
      },
      global: {
        stubs: {
          'v-app-bar': {
            template: '<header class="v-app-bar notebook-app-bar"><slot /></header>'
          },
          'v-icon': true
        }
      }
    })

    expect(wrapper.find('.custom-title-slot').text()).toBe('Custom Slot Title')
    expect(wrapper.find('.custom-append-slot').text()).toBe('Custom Append Content')
  })
})

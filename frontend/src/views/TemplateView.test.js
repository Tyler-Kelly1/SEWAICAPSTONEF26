import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import TemplateView from './TemplateView.vue'
import TemplateApi from '../services/Template.api.js'
import UserApi from '../services/User.api.js'

describe('TemplateView.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(UserApi, 'getUsers').mockResolvedValue([{ userId: 'tyler_dev' }])
    vi.spyOn(TemplateApi, 'getWorkoutTemplates').mockResolvedValue([
      {
        id: 1,
        userId: 'tyler_dev',
        workout_Name: 'Power Upper',
        exerciseTemplates: [{ id: 1, exercise_Name: 'Bench Press' }]
      }
    ])
    vi.spyOn(TemplateApi, 'getExerciseTemplates').mockResolvedValue([
      { id: 1, exercise_Name: 'Bench Press' }
    ])
  })

  it('renders template view with workout templates and allows creating a template', async () => {
    vi.spyOn(TemplateApi, 'createWorkoutTemplate').mockResolvedValue({
      id: 2,
      userId: 'tyler_dev',
      workout_Name: 'Lower Hypertrophy',
      exerciseTemplates: []
    })

    const wrapper = mount(TemplateView, {
      props: {
        currentUserId: 'tyler_dev'
      },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-card-actions': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-chip': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' },
          'v-tabs': { template: '<div><slot /></div>' },
          'v-tab': { template: '<div><slot /></div>' },
          'v-window': { template: '<div><slot /></div>' },
          'v-window-item': { template: '<div><slot /></div>' },
          'v-list': { template: '<div><slot /></div>' },
          'v-list-item': { template: '<div><slot /></div>' },
          'v-list-item-title': { template: '<div><slot /></div>' },
          'v-list-item-subtitle': { template: '<div><slot /></div>' },
          'v-dialog': { template: '<div><slot /></div>' },
          'v-alert': { template: '<div><slot /></div>' },
          ExerciseTemplateList: true,
          CreateTemplateMenu: true
        }
      }
    })

    expect(wrapper.find('[data-testid="template-view"]').exists()).toBe(true)

    wrapper.vm.newWorkoutName = 'Lower Hypertrophy'
    wrapper.vm.selectedExerciseIds = [1]
    await wrapper.vm.handleCreateWorkoutTemplate()

    expect(TemplateApi.createWorkoutTemplate).toHaveBeenCalled()
    expect(wrapper.emitted('templates-updated')).toBeTruthy()
  })

  it('allows editing an existing workout template', async () => {
    vi.spyOn(TemplateApi, 'updateWorkoutTemplate').mockResolvedValue({
      id: 1,
      userId: 'tyler_dev',
      workout_Name: 'Power Upper Renamed',
      exerciseTemplates: [{ id: 1, exercise_Name: 'Bench Press' }]
    })

    const wrapper = mount(TemplateView, {
      props: { currentUserId: 'tyler_dev' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-card-actions': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-chip': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' },
          'v-tabs': { template: '<div><slot /></div>' },
          'v-tab': { template: '<div><slot /></div>' },
          'v-window': { template: '<div><slot /></div>' },
          'v-window-item': { template: '<div><slot /></div>' },
          'v-list': { template: '<div><slot /></div>' },
          'v-list-item': { template: '<div><slot /></div>' },
          'v-list-item-title': { template: '<div><slot /></div>' },
          'v-list-item-subtitle': { template: '<div><slot /></div>' },
          'v-dialog': { template: '<div><slot /></div>' },
          'v-alert': { template: '<div><slot /></div>' },
          ExerciseTemplateList: true,
          CreateTemplateMenu: true
        }
      }
    })

    wrapper.vm.openEditDialog({
      id: 1,
      workout_Name: 'Power Upper',
      exerciseTemplates: [{ id: 1, exercise_Name: 'Bench Press' }]
    })

    expect(wrapper.vm.editingTemplateId).toBe(1)
    expect(wrapper.vm.editWorkoutName).toBe('Power Upper')

    wrapper.vm.editWorkoutName = 'Power Upper Renamed'
    await wrapper.vm.handleSaveEdit()

    expect(TemplateApi.updateWorkoutTemplate).toHaveBeenCalledWith(
      1,
      expect.objectContaining({ workout_Name: 'Power Upper Renamed', userId: 'tyler_dev' })
    )
  })

  it('allows deleting a workout template', async () => {
    vi.spyOn(TemplateApi, 'deleteWorkoutTemplate').mockResolvedValue({ success: true, id: 1 })

    const wrapper = mount(TemplateView, {
      props: { currentUserId: 'tyler_dev' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-card-actions': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-chip': true,
          'v-row': { template: '<div><slot /></div>' },
          'v-col': { template: '<div><slot /></div>' },
          'v-form': { template: '<div><slot /></div>' },
          'v-tabs': { template: '<div><slot /></div>' },
          'v-tab': { template: '<div><slot /></div>' },
          'v-window': { template: '<div><slot /></div>' },
          'v-window-item': { template: '<div><slot /></div>' },
          'v-list': { template: '<div><slot /></div>' },
          'v-list-item': { template: '<div><slot /></div>' },
          'v-list-item-title': { template: '<div><slot /></div>' },
          'v-list-item-subtitle': { template: '<div><slot /></div>' },
          'v-dialog': { template: '<div><slot /></div>' },
          'v-alert': { template: '<div><slot /></div>' },
          ExerciseTemplateList: true,
          CreateTemplateMenu: true
        }
      }
    })

    wrapper.vm.confirmDelete({ id: 1, workout_Name: 'Power Upper' })
    expect(wrapper.vm.deletingTemplate.id).toBe(1)

    await wrapper.vm.handleDelete()
    expect(TemplateApi.deleteWorkoutTemplate).toHaveBeenCalledWith(1)
  })

  it('does not render a user select dropdown and displays current athlete chip', () => {
    const wrapper = mount(TemplateView, {
      props: { currentUserId: 'athlete_jane' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-card-actions': { template: '<div><slot /></div>' },
          'v-chip': { template: '<span data-testid="chip"><slot /></span>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-row': true,
          'v-col': true,
          'v-form': true,
          'v-tabs': true,
          'v-tab': true,
          'v-window': true,
          'v-window-item': true,
          'v-list': true,
          'v-list-item': true,
          'v-list-item-title': true,
          'v-list-item-subtitle': true,
          'v-dialog': true,
          'v-alert': true,
          ExerciseTemplateList: true,
          CreateTemplateMenu: true
        }
      }
    })

    const athleteChip = wrapper.find('[data-testid="current-athlete-chip"]')
    expect(athleteChip.exists()).toBe(true)
    expect(athleteChip.text()).toContain('athlete_jane')
  })

  it('creates workout templates scoped to currentUserId', async () => {
    vi.spyOn(TemplateApi, 'createWorkoutTemplate').mockResolvedValue({
      id: 99,
      userId: 'athlete_2',
      workout_Name: 'Leg Day',
      exerciseTemplates: []
    })

    const wrapper = mount(TemplateView, {
      props: { currentUserId: 'athlete_2' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-card-actions': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-chip': true,
          'v-row': true,
          'v-col': true,
          'v-form': true,
          'v-tabs': true,
          'v-tab': true,
          'v-window': true,
          'v-window-item': true,
          'v-list': true,
          'v-list-item': true,
          'v-list-item-title': true,
          'v-list-item-subtitle': true,
          'v-dialog': true,
          'v-alert': true,
          ExerciseTemplateList: true,
          CreateTemplateMenu: true
        }
      }
    })

    wrapper.vm.newWorkoutName = 'Leg Day'
    wrapper.vm.selectedExerciseIds = [1]
    await wrapper.vm.handleCreateWorkoutTemplate()

    expect(TemplateApi.createWorkoutTemplate).toHaveBeenCalledWith(
      expect.objectContaining({
        userId: 'athlete_2',
        workout_Name: 'Leg Day'
      })
    )
  })

  it('creates exercise templates scoped to currentUserId', async () => {
    vi.spyOn(TemplateApi, 'createExerciseTemplate').mockResolvedValue({
      id: 50,
      userId: 'athlete_2',
      exercise_Name: 'Squat'
    })

    const wrapper = mount(TemplateView, {
      props: { currentUserId: 'athlete_2' },
      global: {
        stubs: {
          'v-card': { template: '<div><slot /></div>' },
          'v-card-item': { template: '<div><slot /></div>' },
          'v-card-title': { template: '<div><slot /></div>' },
          'v-card-subtitle': { template: '<div><slot /></div>' },
          'v-card-text': { template: '<div><slot /></div>' },
          'v-card-actions': { template: '<div><slot /></div>' },
          'v-icon': true,
          'v-divider': true,
          'v-select': true,
          'v-text-field': true,
          'v-btn': true,
          'v-chip': true,
          'v-row': true,
          'v-col': true,
          'v-form': true,
          'v-tabs': true,
          'v-tab': true,
          'v-window': true,
          'v-window-item': true,
          'v-list': true,
          'v-list-item': true,
          'v-list-item-title': true,
          'v-list-item-subtitle': true,
          'v-dialog': true,
          'v-alert': true,
          ExerciseTemplateList: true,
          CreateTemplateMenu: true
        }
      }
    })

    await wrapper.vm.handleCreateExerciseTemplate({
      exercise_Name: 'Squat',
      min_Set: 3,
      max_Set: 5
    })

    expect(TemplateApi.createExerciseTemplate).toHaveBeenCalledWith(
      expect.objectContaining({
        userId: 'athlete_2',
        exercise_Name: 'Squat'
      })
    )
  })
})

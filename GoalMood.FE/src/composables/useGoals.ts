import { ref, type Ref } from 'vue'
import type { Goal } from '@/types/Goal'
import { apiClient } from '@/api/client'

export function useGoals() {
  const loading: Ref<boolean> = ref(false)
  const error: Ref<string | null> = ref(null)

  const createGoal = async (teamMemberId: number, description: string): Promise<Goal | null> => {
    loading.value = true
    error.value = null
    try {
      const goal = await apiClient.post<Goal>('/goals', {
        teamMemberId,
        description,
      })
      return goal
    } catch (e) {
      error.value = 'Failed to create goal'
      console.error('Error creating goal:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  const deleteGoal = async (goalId: number): Promise<boolean> => {
    loading.value = true
    error.value = null
    try {
      await apiClient.delete(`/goals/${goalId}`)
      return true
    } catch (e) {
      error.value = 'Failed to delete goal'
      console.error('Error deleting goal:', e)
      return false
    } finally {
      loading.value = false
    }
  }

  const toggleComplete = async (goalId: number, isCompleted: boolean): Promise<boolean> => {
    loading.value = true
    error.value = null
    try {
      const endpoint = isCompleted ? `/goals/${goalId}/complete` : `/goals/${goalId}/uncomplete`
      await apiClient.put(endpoint, {})
      return true
    } catch (e) {
      error.value = 'Failed to update goal'
      console.error('Error updating goal:', e)
      return false
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    error,
    createGoal,
    deleteGoal,
    toggleComplete,
  }
}

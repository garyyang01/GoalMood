import { ref, type Ref } from 'vue'
import { Mood } from '@/types/TeamMember'
import { apiClient } from '@/api/client'

export function useMoods() {
  const loading: Ref<boolean> = ref(false)
  const error: Ref<string | null> = ref(null)

  const updateMood = async (memberId: number, mood: Mood): Promise<boolean> => {
    loading.value = true
    error.value = null
    try {
      await apiClient.put(`/members/${memberId}/mood`, { mood })
      return true
    } catch (e) {
      error.value = 'Failed to update mood'
      console.error('Error updating mood:', e)
      return false
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    error,
    updateMood,
  }
}

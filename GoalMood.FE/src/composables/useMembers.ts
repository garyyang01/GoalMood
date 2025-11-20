import { ref, type Ref } from 'vue'
import type { TeamMember } from '@/types/TeamMember'
import { apiClient } from '@/api/client'

export function useMembers() {
  const members: Ref<TeamMember[]> = ref([])
  const loading: Ref<boolean> = ref(false)
  const error: Ref<string | null> = ref(null)

  const fetchMembers = async () => {
    loading.value = true
    error.value = null
    try {
      members.value = await apiClient.get<TeamMember[]>('/members')
    } catch (e) {
      error.value = 'Failed to load team members'
      console.error('Error fetching members:', e)
    } finally {
      loading.value = false
    }
  }

  return {
    members,
    loading,
    error,
    fetchMembers,
  }
}

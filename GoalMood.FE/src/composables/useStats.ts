import { ref, type Ref } from 'vue'
import type { Stats } from '@/types/Stats'
import { apiClient } from '@/api/client'

export function useStats() {
  const stats: Ref<Stats | null> = ref(null)
  const loading: Ref<boolean> = ref(false)
  const error: Ref<string | null> = ref(null)

  const fetchStats = async () => {
    loading.value = true
    error.value = null
    try {
      stats.value = await apiClient.get<Stats>('/stats')
    } catch (e) {
      error.value = 'Failed to load statistics'
      console.error('Error fetching stats:', e)
    } finally {
      loading.value = false
    }
  }

  return {
    stats,
    loading,
    error,
    fetchStats,
  }
}

<script setup lang="ts">
import { computed } from 'vue'
import type { Stats } from '@/types/Stats'
import { Mood } from '@/types/TeamMember'

const props = defineProps<{
  stats: Stats | null
}>()

const moodEmojis: Record<Mood, string> = {
  [Mood.Happy]: '😀',
  [Mood.Content]: '😊',
  [Mood.Neutral]: '😐',
  [Mood.Sad]: '😞',
  [Mood.Stressed]: '😤',
}

const moodLabels: Record<Mood, string> = {
  [Mood.Happy]: 'Happy',
  [Mood.Content]: 'Content',
  [Mood.Neutral]: 'Neutral',
  [Mood.Sad]: 'Sad',
  [Mood.Stressed]: 'Stressed',
}

const moodDistributionArray = computed(() => {
  if (!props.stats?.moodDistribution) return []

  const result = Object.entries(props.stats.moodDistribution)
    .filter(([_, count]) => count > 0)
    .map(([moodKey, count]) => {
      // Handle both numeric keys ("1", "2") and string keys ("Happy", "Content")
      let moodNum: Mood

      // Try parsing as number first
      const parsed = Number(moodKey)
      if (!isNaN(parsed)) {
        moodNum = parsed as Mood
      } else {
        // If it's a string name, convert to enum value
        const moodName = moodKey as keyof typeof Mood
        moodNum = Mood[moodName] as Mood
      }

      return {
        mood: moodNum,
        count,
        emoji: moodEmojis[moodNum],
        label: moodLabels[moodNum],
      }
    })
    .filter(item => item.emoji && item.label) // Filter out invalid entries

  return result
})

const totalPeople = computed(() => {
  return moodDistributionArray.value.reduce((sum, item) => sum + item.count, 0)
})
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">Team Statistics</h2>

      <div v-if="!stats" class="text-center text-gray-500">
        No statistics available
      </div>

      <div v-else class="space-y-4">
        <!-- Completion Percentage -->
        <div class="stats shadow w-full">
          <div class="stat">
            <div class="stat-title">Goal Completion</div>
            <div class="stat-value">{{ stats.completionPercentage.toFixed(1) }}%</div>
            <div class="stat-desc">Today's progress</div>
          </div>
        </div>

        <!-- Mood Distribution -->
        <div>
          <h3 class="font-semibold mb-2">Mood Distribution</h3>
          <div v-if="totalPeople === 0" class="text-gray-500 text-sm">
            No mood data available
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="item in moodDistributionArray"
              :key="item.mood"
              class="flex items-center gap-4 bg-base-200 p-4 rounded-lg"
            >
              <div class="text-5xl" style="min-width: 3rem; line-height: 1;">
                {{ item.emoji || '❓' }}
              </div>
              <div class="flex-1">
                <div class="text-2xl font-bold">{{ item.count }}</div>
                <div class="text-sm opacity-70">{{ item.label || 'Unknown' }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

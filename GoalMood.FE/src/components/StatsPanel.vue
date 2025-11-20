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

  return Object.entries(props.stats.moodDistribution)
    .filter(([_, count]) => count > 0)
    .map(([mood, count]) => ({
      mood: Number(mood) as Mood,
      count,
      emoji: moodEmojis[Number(mood) as Mood],
      label: moodLabels[Number(mood) as Mood],
    }))
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
          <div v-else class="flex flex-wrap gap-2">
            <div
              v-for="item in moodDistributionArray"
              :key="item.mood"
              class="badge badge-lg gap-1"
            >
              <span>{{ item.emoji }}</span>
              <span>{{ item.count }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

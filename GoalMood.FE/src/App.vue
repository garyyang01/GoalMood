<script setup lang="ts">
import { onMounted } from 'vue'
import MemberCard from './components/MemberCard.vue'
import GoalInput from './components/GoalInput.vue'
import MoodSelector from './components/MoodSelector.vue'
import StatsPanel from './components/StatsPanel.vue'
import { useMembers } from './composables/useMembers'
import { useStats } from './composables/useStats'

const { members, loading, error, fetchMembers } = useMembers()
const { stats, fetchStats } = useStats()

onMounted(() => {
  fetchMembers()
  fetchStats()
})

const handleGoalCreated = () => {
  fetchMembers()
  fetchStats()
}

const handleGoalUpdated = () => {
  fetchMembers()
  fetchStats()
}

const handleMoodUpdated = () => {
  fetchMembers()
  fetchStats()
}
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-4xl font-bold text-center mb-8">Team Goal & Mood Tracker</h1>

    <!-- Stats Panel -->
    <div class="mb-8 max-w-2xl mx-auto">
      <StatsPanel :stats="stats" />
    </div>

    <!-- Input Forms -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <GoalInput :members="members" @goal-created="handleGoalCreated" />
      <MoodSelector :members="members" @mood-updated="handleMoodUpdated" />
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="flex justify-center">
      <span class="loading loading-spinner loading-lg"></span>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="alert alert-error max-w-2xl mx-auto">
      <span>{{ error }}</span>
    </div>

    <!-- Empty state -->
    <div v-else-if="members.length === 0" class="text-center text-gray-500">
      No team members found
    </div>

    <!-- Members grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <MemberCard
        v-for="member in members"
        :key="member.id"
        :member="member"
        @goal-updated="handleGoalUpdated"
      />
    </div>
  </div>
</template>

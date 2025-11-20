<script setup lang="ts">
import { ref } from 'vue'
import type { TeamMember } from '@/types/TeamMember'
import { Mood } from '@/types/TeamMember'
import { useMoods } from '@/composables/useMoods'

const props = defineProps<{
  members: TeamMember[]
}>()

const emit = defineEmits<{
  moodUpdated: []
}>()

const { updateMood, loading, error } = useMoods()

const selectedMemberId = ref<number | null>(null)
const selectedMood = ref<Mood | null>(null)
const validationError = ref('')

const moods = [
  { value: Mood.Happy, emoji: '😀', label: 'Happy' },
  { value: Mood.Content, emoji: '😊', label: 'Content' },
  { value: Mood.Neutral, emoji: '😐', label: 'Neutral' },
  { value: Mood.Sad, emoji: '😞', label: 'Sad' },
  { value: Mood.Stressed, emoji: '😤', label: 'Stressed' },
]

const handleSubmit = async () => {
  // Validate
  validationError.value = ''

  if (!selectedMemberId.value) {
    validationError.value = 'Please select a team member'
    return
  }

  if (selectedMood.value === null) {
    validationError.value = 'Please select a mood'
    return
  }

  // Update mood
  const success = await updateMood(selectedMemberId.value, selectedMood.value)
  if (success) {
    // Reset form
    selectedMemberId.value = null
    selectedMood.value = null
    emit('moodUpdated')
  }
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">Update Mood</h2>

      <form @submit.prevent="handleSubmit" class="space-y-4" role="form" aria-label="Update mood form">
        <!-- Team member selection -->
        <div class="form-control">
          <label for="mood-member-select" class="label">
            <span class="label-text">Team Member</span>
          </label>
          <select
            id="mood-member-select"
            v-model="selectedMemberId"
            class="select select-bordered w-full"
            aria-label="Select team member to update mood"
            aria-required="true"
          >
            <option :value="null" disabled>Select a team member</option>
            <option v-for="member in members" :key="member.id" :value="member.id">
              {{ member.name }}
            </option>
          </select>
        </div>

        <!-- Mood selection -->
        <div class="form-control">
          <label id="mood-label" class="label">
            <span class="label-text">Mood</span>
          </label>
          <div
            class="btn-group w-full justify-center"
            role="group"
            aria-labelledby="mood-label"
            aria-required="true"
          >
            <button
              v-for="mood in moods"
              :key="mood.value"
              type="button"
              @click="selectedMood = mood.value"
              class="btn"
              :class="{ 'btn-active': selectedMood === mood.value }"
              :aria-label="`${mood.label} mood`"
              :aria-pressed="selectedMood === mood.value"
              role="button"
            >
              <span class="text-2xl" aria-hidden="true">{{ mood.emoji }}</span>
              <span class="sr-only">{{ mood.label }}</span>
            </button>
          </div>
        </div>

        <!-- Error messages -->
        <div v-if="validationError || error" class="alert alert-error" role="alert" aria-live="assertive">
          <span>{{ validationError || error }}</span>
        </div>

        <!-- Submit button -->
        <div class="card-actions justify-end">
          <button
            type="submit"
            class="btn btn-primary"
            :disabled="loading"
            :aria-busy="loading"
            aria-label="Submit mood update"
          >
            <span v-if="loading" class="loading loading-spinner" aria-hidden="true"></span>
            <span>{{ loading ? 'Updating...' : 'Update Mood' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

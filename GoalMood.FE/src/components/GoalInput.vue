<script setup lang="ts">
import { ref } from 'vue'
import type { TeamMember } from '@/types/TeamMember'
import { useGoals } from '@/composables/useGoals'

const props = defineProps<{
  members: TeamMember[]
}>()

const emit = defineEmits<{
  goalCreated: []
}>()

const { createGoal, loading, error } = useGoals()

const selectedMemberId = ref<number | null>(null)
const description = ref('')
const validationError = ref('')

const handleSubmit = async () => {
  // Validate
  validationError.value = ''

  if (!selectedMemberId.value) {
    validationError.value = 'Please select a team member'
    return
  }

  if (!description.value.trim()) {
    validationError.value = 'Please enter a goal description'
    return
  }

  if (description.value.length > 500) {
    validationError.value = 'Description cannot exceed 500 characters'
    return
  }

  // Create goal
  const success = await createGoal(selectedMemberId.value, description.value)
  if (success) {
    // Reset form
    selectedMemberId.value = null
    description.value = ''
    emit('goalCreated')
  }
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">Add New Goal</h2>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <!-- Team member selection -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Team Member</span>
          </label>
          <select
            v-model="selectedMemberId"
            class="select select-bordered w-full"
          >
            <option :value="null" disabled>Select a team member</option>
            <option v-for="member in members" :key="member.id" :value="member.id">
              {{ member.name }}
            </option>
          </select>
        </div>

        <!-- Goal description -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Goal Description</span>
            <span class="label-text-alt">{{ description.length }}/500</span>
          </label>
          <textarea
            v-model="description"
            class="textarea textarea-bordered h-24"
            placeholder="Enter goal description..."
            maxlength="500"
          ></textarea>
        </div>

        <!-- Error messages -->
        <div v-if="validationError || error" class="alert alert-error">
          <span>{{ validationError || error }}</span>
        </div>

        <!-- Submit button -->
        <div class="card-actions justify-end">
          <button
            type="submit"
            class="btn btn-primary"
            :disabled="loading"
          >
            <span v-if="loading" class="loading loading-spinner"></span>
            <span v-else>Add Goal</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

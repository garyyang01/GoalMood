<script setup lang="ts">
import { ref } from 'vue'
import type { TeamMember } from '@/types/TeamMember'
import { useGoals } from '@/composables/useGoals'

const props = defineProps<{
  member: TeamMember
}>()

const emit = defineEmits<{
  goalUpdated: []
}>()

const { deleteGoal, toggleComplete } = useGoals()

const showDeleteModal = ref(false)
const goalToDelete = ref<number | null>(null)

const handleToggleComplete = async (goalId: number, currentStatus: boolean) => {
  const success = await toggleComplete(goalId, !currentStatus)
  if (success) {
    emit('goalUpdated')
  }
}

const confirmDelete = (goalId: number) => {
  goalToDelete.value = goalId
  showDeleteModal.value = true
}

const handleDelete = async () => {
  if (goalToDelete.value) {
    const success = await deleteGoal(goalToDelete.value)
    if (success) {
      showDeleteModal.value = false
      goalToDelete.value = null
      emit('goalUpdated')
    }
  }
}

const cancelDelete = () => {
  showDeleteModal.value = false
  goalToDelete.value = null
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">{{ member.name }}</h2>
      <div class="badge badge-lg">{{ member.moodEmoji }}</div>

      <div class="space-y-2 mt-4">
        <div v-for="goal in member.goals" :key="goal.id" class="flex items-center gap-2">
          <input
            type="checkbox"
            class="checkbox"
            :checked="goal.isCompleted"
            @change="handleToggleComplete(goal.id, goal.isCompleted)"
          />
          <span
            :class="{ 'line-through text-gray-500': goal.isCompleted }"
            class="flex-1"
          >
            {{ goal.description }}
          </span>
          <button
            @click="confirmDelete(goal.id)"
            class="btn btn-ghost btn-xs"
            aria-label="Delete goal"
          >
            🗑️
          </button>
        </div>
        <div v-if="member.goals.length === 0" class="text-gray-500 text-sm">
          No goals for today
        </div>
      </div>

      <div class="card-actions justify-end mt-4">
        <div class="badge">{{ member.completedCount }}/{{ member.totalCount }} completed</div>
      </div>
    </div>
  </div>

  <!-- Delete confirmation modal -->
  <dialog :open="showDeleteModal" class="modal" @click.self="cancelDelete">
    <div class="modal-box">
      <h3 class="font-bold text-lg">Confirm Delete</h3>
      <p class="py-4">Are you sure you want to delete this goal?</p>
      <div class="modal-action">
        <button @click="cancelDelete" class="btn">Cancel</button>
        <button @click="handleDelete" class="btn btn-error">Delete</button>
      </div>
    </div>
  </dialog>
</template>

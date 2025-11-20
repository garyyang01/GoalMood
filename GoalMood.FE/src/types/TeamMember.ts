import type { Goal } from './Goal'

export enum Mood {
  Happy = 1,
  Content = 2,
  Neutral = 3,
  Sad = 4,
  Stressed = 5
}

export interface TeamMember {
  id: number
  name: string
  currentMood: Mood
  moodEmoji: string
  goals: Goal[]
  completedCount: number
  totalCount: number
}

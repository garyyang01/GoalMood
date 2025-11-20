import type { Mood } from './TeamMember'

export interface Stats {
  completionPercentage: number
  moodDistribution: Record<Mood, number>
}

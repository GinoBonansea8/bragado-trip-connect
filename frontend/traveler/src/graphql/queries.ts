import { gql } from '@apollo/client'

export const HEALTH_QUERY = gql`
  query Health {
    health {
      status
      checkedAt
    }
  }
`

export interface HealthQueryResult {
  health: {
    status: string
    checkedAt: string
  } | null
}

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

export const ROUTES_QUERY = gql`
  query Routes {
    routes {
      origin
      destination
    }
  }
`

export interface RoutesResult {
  routes: { origin: string; destination: string }[]
}

// The date argument is a LocalDate, which is what HotChocolate maps a C#
// DateOnly to.
export const SEARCH_QUERY = gql`
  query SearchDepartures($origin: String!, $destination: String!, $date: LocalDate!) {
    searchDepartures(origin: $origin, destination: $destination, date: $date) {
      companyCuit
      companyName
      departureTime
      durationMinutes
      price
    }
  }
`

export interface DepartureOption {
  companyCuit: string
  companyName: string
  departureTime: string
  durationMinutes: number
  price: number
}

export interface SearchResult {
  searchDepartures: DepartureOption[]
}

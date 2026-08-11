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

export const FORM_OPTIONS_QUERY = gql`
  query FormOptions {
    companies {
      cuit
      name
    }
    routes {
      origin
      destination
    }
  }
`

export interface FormOptionsResult {
  companies: { cuit: string; name: string }[]
  routes: { origin: string; destination: string }[]
}

export const DEPARTURES_QUERY = gql`
  query Departures($companyCuit: String!) {
    departuresByCompany(companyCuit: $companyCuit) {
      routeOrigin
      routeDestination
      date
      time
      price
      durationMinutes
    }
  }
`

export interface Departure {
  routeOrigin: string
  routeDestination: string
  date: string
  time: string
  price: number
  durationMinutes: number
}

export interface DeparturesResult {
  departuresByCompany: Departure[]
}

export const PUBLISH_DEPARTURE_MUTATION = gql`
  mutation PublishDeparture($input: PublishDepartureInput!) {
    publishDeparture(input: $input) {
      routeOrigin
      routeDestination
      date
      time
    }
  }
`

import { useQuery } from '@apollo/client/react'
import { HEALTH_QUERY, type HealthQueryResult } from './graphql/queries'

function App() {
  const { data, loading, error } = useQuery<HealthQueryResult>(HEALTH_QUERY)

  return (
    <main>
      <h1>Bragado Trip Connect</h1>
      <p>
        Comparing Bragado ↔ Buenos Aires trips across 21900, Santorini
        Turismo, Chevallier, and Trenes Argentinos.
      </p>

      <section>
        <h2>Backend status</h2>
        {loading && <p>Checking connection to the API…</p>}
        {error && <p>Could not reach the API: {error.message}</p>}
        {data?.health && (
          <p>
            Status: <strong>{data.health.status}</strong> (checked at{' '}
            {new Date(data.health.checkedAt).toLocaleString()})
          </p>
        )}
      </section>
    </main>
  )
}

export default App

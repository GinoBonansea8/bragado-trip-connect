import { useQuery } from '@apollo/client/react'
import { HEALTH_QUERY, type HealthQueryResult } from './graphql/queries'

function ApiStatus() {
  const { data, loading, error } = useQuery<HealthQueryResult>(HEALTH_QUERY)

  if (loading) return <p className="status">Checking the API…</p>
  if (error) return <p className="status status-error">API unreachable: {error.message}</p>

  return <p className="status status-ok">API reachable — reported “{data?.health?.status}”.</p>
}

export default ApiStatus

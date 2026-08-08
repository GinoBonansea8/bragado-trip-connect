import { useQuery } from '@apollo/client/react'
import { HEALTH_QUERY, type HealthQueryResult } from './graphql/queries'

function ApiStatus() {
  const { data, loading, error } = useQuery<HealthQueryResult>(HEALTH_QUERY)

  if (loading) return <p className="status">Verificando la conexión con la API…</p>
  if (error) return <p className="status status-error">No se pudo conectar con la API: {error.message}</p>

  return <p className="status status-ok">API conectada — respondió “{data?.health?.status}”.</p>
}

export default ApiStatus

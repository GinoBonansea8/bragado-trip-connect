import { useState, type FormEvent } from 'react'
import { useQuery } from '@apollo/client/react'
import ApiStatus from './ApiStatus'
import { arrivalOf, formatDuration, formatPrice, formatTime, todayIsoDate } from './format'
import {
  ROUTES_QUERY,
  SEARCH_QUERY,
  type RoutesResult,
  type SearchResult,
} from './graphql/queries'

type SortKey = 'departure' | 'price' | 'duration'

interface Search {
  origin: string
  destination: string
  date: string
}

function App() {
  const [origin, setOrigin] = useState('')
  const [destination, setDestination] = useState('')
  const [date, setDate] = useState(todayIsoDate())
  const [search, setSearch] = useState<Search | null>(null)
  const [sortBy, setSortBy] = useState<SortKey>('departure')

  const { data: routeData, loading: loadingRoutes } = useQuery<RoutesResult>(ROUTES_QUERY)
  const routes = routeData?.routes ?? []

  const from = origin || routes[0]?.origin || ''
  const reachable = routes.filter(route => route.origin === from).map(route => route.destination)
  const to = reachable.includes(destination) ? destination : reachable[0] ?? ''

  const { data, loading, error } = useQuery<SearchResult>(SEARCH_QUERY, {
    variables: search ?? { origin: '', destination: '', date: '' },
    skip: !search,
  })

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setSearch({ origin: from, destination: to, date })
  }

  const found = data?.searchDepartures ?? []

  const cheapest = Math.min(...found.map(option => option.price))
  const quickest = Math.min(...found.map(option => option.durationMinutes))

  const sorted = [...found].sort((left, right) => {
    if (sortBy === 'price') return left.price - right.price
    if (sortBy === 'duration') return left.durationMinutes - right.durationMinutes
    return left.departureTime.localeCompare(right.departureTime)
  })

  return (
    <>
      <header>
        <span className="brand">Bragado Trip Connect</span>
        <span className="badge">Viajeros</span>
      </header>

      <main>
        <h1>Encontrá tu viaje</h1>
        <p>
          Todas las salidas entre Bragado y Buenos Aires — 21900, Santorini
          Turismo, Chevallier y Trenes Argentinos — en un solo lugar, una al
          lado de la otra.
        </p>

        <form className="panel" onSubmit={handleSubmit}>
          <div className="row">
            <div className="field">
              <label htmlFor="origin">Desde</label>
              <select
                id="origin"
                value={from}
                onChange={event => setOrigin(event.target.value)}
                disabled={loadingRoutes}
                required
              >
                {[...new Set(routes.map(route => route.origin))].map(stop => (
                  <option key={stop} value={stop}>{stop}</option>
                ))}
              </select>
            </div>

            <div className="field">
              <label htmlFor="destination">Hasta</label>
              <select
                id="destination"
                value={to}
                onChange={event => setDestination(event.target.value)}
                disabled={loadingRoutes}
                required
              >
                {reachable.map(stop => <option key={stop} value={stop}>{stop}</option>)}
              </select>
            </div>

            <div className="field">
              <label htmlFor="date">Fecha</label>
              <input
                id="date"
                type="date"
                value={date}
                onChange={event => setDate(event.target.value)}
                required
              />
            </div>
          </div>

          <button type="submit" disabled={loadingRoutes}>Buscar salidas</button>
        </form>

        <h2>Salidas</h2>

        {!search && (
          <p className="placeholder">
            Elegí un recorrido y una fecha para ver todas las opciones.
          </p>
        )}

        {search && loading && <p className="placeholder">Buscando salidas…</p>}

        {search && error && (
          <p className="feedback feedback-error">
            No se pudo hacer la búsqueda: {error.message}
          </p>
        )}

        {search && !loading && !error && found.length === 0 && (
          <p className="placeholder">
            Ninguna empresa publicó salidas de {search.origin} a {search.destination} para
            esa fecha.
          </p>
        )}

        {found.length > 0 && (
          <>
            <div className="sort">
              <span>Ordenar por</span>
              {([
                ['departure', 'hora de salida'],
                ['price', 'precio'],
                ['duration', 'duración'],
              ] as [SortKey, string][]).map(([key, label]) => (
                <button
                  key={key}
                  type="button"
                  className={sortBy === key ? 'chip chip-on' : 'chip'}
                  onClick={() => setSortBy(key)}
                >
                  {label}
                </button>
              ))}
            </div>

            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Empresa</th>
                    <th>Sale</th>
                    <th>Llega (aprox.)</th>
                    <th>Duración</th>
                    <th>Precio</th>
                  </tr>
                </thead>
                <tbody>
                  {sorted.map(option => {
                    const { arrival, daysLater } = arrivalOf(option.departureTime, option.durationMinutes)

                    return (
                      <tr key={`${option.companyCuit}-${option.departureTime}`}>
                        <td>{option.companyName}</td>
                        <td>{formatTime(option.departureTime)}</td>
                        <td>
                          {arrival}
                          {daysLater > 0 && <span className="next-day"> +{daysLater}</span>}
                        </td>
                        <td>
                          {formatDuration(option.durationMinutes)}
                          {option.durationMinutes === quickest && <span className="tag">más rápido</span>}
                        </td>
                        <td>
                          {formatPrice(option.price)}
                          {option.price === cheapest && <span className="tag">más barato</span>}
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          </>
        )}

        <h2>Mis reservas</h2>
        <p className="placeholder">
          Los viajes que reserves se van a listar acá.
        </p>

        <ApiStatus />
      </main>
    </>
  )
}

export default App

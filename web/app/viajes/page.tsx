import Link from 'next/link'
import { requireRole } from '@/lib/auth'
import { arrivalOf, formatDate, formatDuration, formatPrice, formatTime, todayIsoDate } from '@/lib/format'
import AppHeader from '../AppHeader'

type SortKey = 'departure' | 'price' | 'duration'

const sortOptions: [SortKey, string][] = [
  ['departure', 'hora de salida'],
  ['price', 'precio'],
  ['duration', 'duración'],
]

interface Departure {
  id: number
  date: string
  time: string
  price: number
  duration_minutes: number
  company: { name: string }
  route: { origin: string, destination: string }
}

// The search lives in the URL (?ruta=1&fecha=2026-09-22&orden=price), so it
// works without JavaScript and a search can be shared as a link.
export default async function TravelerPage({ searchParams }: PageProps<'/viajes'>) {
  const { supabase, email } = await requireRole('traveler')
  const params = await searchParams

  const routeId = typeof params.ruta === 'string' ? params.ruta : ''
  const date = typeof params.fecha === 'string' ? params.fecha : ''
  const sortBy = sortOptions.some(([key]) => key === params.orden) ? params.orden as SortKey : 'departure'
  const today = todayIsoDate()

  let query = supabase
    .from('schedules')
    .select('id, date, time, price, duration_minutes, company:companies (name), route:routes (origin, destination)')

  // Without a date, list everything still to come.
  query = date ? query.eq('date', date) : query.gte('date', today)
  if (routeId) query = query.eq('route_id', routeId)

  const [{ data: routes }, { data, error }] = await Promise.all([
    supabase.from('routes').select('id, origin, destination').order('id'),
    query,
  ])

  const found = (data ?? []) as unknown as Departure[]

  // "Cheapest" and "quickest" only mean something among trips on the same
  // route and day.
  const comparing = Boolean(routeId && date)
  const cheapest = Math.min(...found.map(option => option.price))
  const quickest = Math.min(...found.map(option => option.duration_minutes))

  const sorted = [...found].sort((left, right) => {
    if (sortBy === 'price') return left.price - right.price
    if (sortBy === 'duration') return left.duration_minutes - right.duration_minutes
    return left.date.localeCompare(right.date) || left.time.localeCompare(right.time)
  })

  function sortLink(key: SortKey) {
    const next = new URLSearchParams()
    if (routeId) next.set('ruta', routeId)
    if (date) next.set('fecha', date)
    next.set('orden', key)

    return `/viajes?${next}`
  }

  return (
    <>
      <AppHeader badge="Viajeros" email={email} />

      <main>
        <h1>Encontrá tu viaje</h1>
        <p>
          Todas las salidas entre Bragado y Buenos Aires — 21900, Santorini
          Turismo, Chevallier y Trenes Argentinos — en un solo lugar, una al
          lado de la otra.
        </p>

        <form className="panel" action="/viajes">
          <div className="row">
            <div className="field">
              <label htmlFor="ruta">Recorrido</label>
              <select id="ruta" name="ruta" defaultValue={routeId}>
                <option value="">Todos</option>
                {routes?.map(route => (
                  <option key={route.id} value={route.id}>{route.origin} → {route.destination}</option>
                ))}
              </select>
            </div>

            <div className="field">
              <label htmlFor="fecha">Fecha</label>
              <input id="fecha" name="fecha" type="date" min={today} defaultValue={date} />
            </div>
          </div>

          <button type="submit">Buscar salidas</button>
        </form>

        <h2>{date ? `Salidas del ${formatDate(date)}` : 'Próximas salidas'}</h2>

        {error && (
          <p className="feedback feedback-error">
            No se pudo hacer la búsqueda: {error.message}
          </p>
        )}

        {!error && found.length === 0 && (
          <p className="placeholder">
            Ninguna empresa publicó salidas para esa búsqueda todavía.
          </p>
        )}

        {found.length > 0 && (
          <>
            <div className="sort">
              <span>Ordenar por</span>
              {sortOptions.map(([key, label]) => (
                <Link key={key} href={sortLink(key)} className={sortBy === key ? 'chip chip-on' : 'chip'}>
                  {label}
                </Link>
              ))}
            </div>

            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    {!date && <th>Fecha</th>}
                    {!routeId && <th>Recorrido</th>}
                    <th>Empresa</th>
                    <th>Sale</th>
                    <th>Llega (aprox.)</th>
                    <th>Duración</th>
                    <th>Precio</th>
                  </tr>
                </thead>
                <tbody>
                  {sorted.map(option => {
                    const { arrival, daysLater } = arrivalOf(option.time, option.duration_minutes)

                    return (
                      <tr key={option.id}>
                        {!date && <td>{formatDate(option.date)}</td>}
                        {!routeId && <td>{option.route.origin} → {option.route.destination}</td>}
                        <td>{option.company.name}</td>
                        <td>{formatTime(option.time)}</td>
                        <td>
                          {arrival}
                          {daysLater > 0 && <span className="next-day"> +{daysLater}</span>}
                        </td>
                        <td>
                          {formatDuration(option.duration_minutes)}
                          {comparing && option.duration_minutes === quickest && <span className="tag">más rápido</span>}
                        </td>
                        <td>
                          {formatPrice(option.price)}
                          {comparing && option.price === cheapest && <span className="tag">más barato</span>}
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          </>
        )}
      </main>
    </>
  )
}

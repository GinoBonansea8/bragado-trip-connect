import { useState, type FormEvent } from 'react'
import { useMutation, useQuery } from '@apollo/client/react'
import ApiStatus from './ApiStatus'
import { formatDate, formatDuration, formatPrice, formatTime, minutesBetween } from './format'
import {
  DEPARTURES_QUERY,
  FORM_OPTIONS_QUERY,
  PUBLISH_DEPARTURE_MUTATION,
  type DeparturesResult,
  type FormOptionsResult,
} from './graphql/queries'

function App() {
  const [chosenCuit, setChosenCuit] = useState('')
  const [origin, setOrigin] = useState('')
  const [destination, setDestination] = useState('')
  const [date, setDate] = useState('')
  const [departureTime, setDepartureTime] = useState('')
  const [arrivalTime, setArrivalTime] = useState('')
  const [price, setPrice] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [published, setPublished] = useState(false)

  const { data: options, loading: loadingOptions } = useQuery<FormOptionsResult>(FORM_OPTIONS_QUERY)

  const companies = options?.companies ?? []
  const routes = options?.routes ?? []

  // Until the operator picks one, the form acts on the first of the list —
  // the same one the select shows.
  const companyCuit = chosenCuit || companies[0]?.cuit || ''
  const from = origin || routes[0]?.origin || ''
  const reachable = routes.filter(route => route.origin === from).map(route => route.destination)
  const to = reachable.includes(destination) ? destination : reachable[0] ?? ''

  const { data: departures } = useQuery<DeparturesResult>(DEPARTURES_QUERY, {
    variables: { companyCuit },
    skip: !companyCuit,
  })

  const [publishDeparture, { loading: publishing }] = useMutation(PUBLISH_DEPARTURE_MUTATION, {
    refetchQueries: [{ query: DEPARTURES_QUERY, variables: { companyCuit } }],
  })

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setPublished(false)

    try {
      await publishDeparture({
        variables: {
          input: {
            companyCuit,
            origin: from,
            destination: to,
            date,
            departureTime: `${departureTime}:00`,
            durationMinutes: minutesBetween(departureTime, arrivalTime),
            price: Number(price),
          },
        },
      })

      setPublished(true)
      setDepartureTime('')
      setArrivalTime('')
      setPrice('')
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No se pudo publicar la salida.')
    }
  }

  const publishedDepartures = departures?.departuresByCompany ?? []
  const timesEntered = Boolean(departureTime && arrivalTime)
  const arrivesNextDay = timesEntered && arrivalTime <= departureTime

  return (
    <>
      <header>
        <span className="brand">Bragado Trip Connect</span>
        <span className="badge">Empresas</span>
      </header>

      <main>
        <h1>Publicá una salida</h1>
        <p>
          Publicá tus horarios y precios para que los viajeros los encuentren
          junto a los del resto de las empresas que hacen la ruta.
        </p>

        <form className="panel" onSubmit={handleSubmit}>
          <div className="field">
            <label htmlFor="company">Empresa</label>
            <select
              id="company"
              value={companyCuit}
              onChange={event => setChosenCuit(event.target.value)}
              disabled={loadingOptions}
              required
            >
              {companies.map(company => (
                <option key={company.cuit} value={company.cuit}>{company.name}</option>
              ))}
            </select>
          </div>

          <div className="row">
            <div className="field">
              <label htmlFor="origin">Desde</label>
              <select
                id="origin"
                value={from}
                onChange={event => setOrigin(event.target.value)}
                disabled={loadingOptions}
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
                disabled={loadingOptions}
                required
              >
                {reachable.map(stop => <option key={stop} value={stop}>{stop}</option>)}
              </select>
            </div>
          </div>

          <div className="row">
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

            <div className="field">
              <label htmlFor="departure">Sale</label>
              <input
                id="departure"
                type="time"
                value={departureTime}
                onChange={event => setDepartureTime(event.target.value)}
                required
              />
            </div>

            <div className="field">
              <label htmlFor="arrival">Llega (aprox.)</label>
              <input
                id="arrival"
                type="time"
                value={arrivalTime}
                onChange={event => setArrivalTime(event.target.value)}
                required
              />
            </div>
          </div>

          <div className="field">
            <label htmlFor="price">Precio (ARS)</label>
            <input
              id="price"
              type="number"
              min="0"
              step="0.01"
              value={price}
              onChange={event => setPrice(event.target.value)}
              required
            />
          </div>

          {timesEntered && (
            <p className="hint">
              Duración del viaje: {formatDuration(minutesBetween(departureTime, arrivalTime))}
              {arrivesNextDay && ' — llega al día siguiente'}
            </p>
          )}

          <button type="submit" disabled={publishing || loadingOptions}>
            {publishing ? 'Publicando…' : 'Publicar salida'}
          </button>

          {error && <p className="feedback feedback-error">{error}</p>}
          {published && <p className="feedback feedback-ok">Salida publicada.</p>}
        </form>

        <h2>Salidas publicadas</h2>
        {publishedDepartures.length === 0 ? (
          <p className="placeholder">
            Las salidas que publiques se van a listar acá.
          </p>
        ) : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Fecha</th>
                  <th>Ruta</th>
                  <th>Sale</th>
                  <th>Duración</th>
                  <th>Precio</th>
                </tr>
              </thead>
              <tbody>
                {publishedDepartures.map(departure => (
                  <tr key={`${departure.date}-${departure.time}-${departure.routeOrigin}-${departure.routeDestination}`}>
                    <td>{formatDate(departure.date)}</td>
                    <td>{departure.routeOrigin} → {departure.routeDestination}</td>
                    <td>{formatTime(departure.time)}</td>
                    <td>{formatDuration(departure.durationMinutes)}</td>
                    <td>{formatPrice(departure.price)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        <ApiStatus />
      </main>
    </>
  )
}

export default App

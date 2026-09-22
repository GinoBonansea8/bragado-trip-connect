'use client'

import { useActionState, useState } from 'react'
import { formatDuration, minutesBetween } from '@/lib/format'
import { publishDeparture } from './actions'

export interface RouteOption {
  id: number
  origin: string
  destination: string
}

export default function PublishForm({ routes }: { routes: RouteOption[] }) {
  const [state, action, publishing] = useActionState(publishDeparture, null)
  const [origin, setOrigin] = useState(routes[0]?.origin ?? '')
  const [departureTime, setDepartureTime] = useState('')
  const [arrivalTime, setArrivalTime] = useState('')

  const reachable = routes.filter(route => route.origin === origin)
  const timesEntered = Boolean(departureTime && arrivalTime)
  const arrivesNextDay = timesEntered && arrivalTime <= departureTime

  return (
    // React clears the inputs after each submit, so the times held in state
    // are reset with them.
    <form
      className="panel"
      action={action}
      onSubmit={() => { setDepartureTime(''); setArrivalTime('') }}
    >
      <div className="row">
        <div className="field">
          <label htmlFor="origin">Desde</label>
          <select id="origin" value={origin} onChange={event => setOrigin(event.target.value)} required>
            {[...new Set(routes.map(route => route.origin))].map(stop => (
              <option key={stop} value={stop}>{stop}</option>
            ))}
          </select>
        </div>

        <div className="field">
          <label htmlFor="routeId">Hasta</label>
          <select id="routeId" name="routeId" key={origin} required>
            {reachable.map(route => <option key={route.id} value={route.id}>{route.destination}</option>)}
          </select>
        </div>
      </div>

      <div className="row">
        <div className="field">
          <label htmlFor="date">Fecha</label>
          <input id="date" name="date" type="date" required />
        </div>

        <div className="field">
          <label htmlFor="departureTime">Sale</label>
          <input
            id="departureTime"
            name="departureTime"
            type="time"
            value={departureTime}
            onChange={event => setDepartureTime(event.target.value)}
            required
          />
        </div>

        <div className="field">
          <label htmlFor="arrivalTime">Llega (aprox.)</label>
          <input
            id="arrivalTime"
            name="arrivalTime"
            type="time"
            value={arrivalTime}
            onChange={event => setArrivalTime(event.target.value)}
            required
          />
        </div>
      </div>

      <div className="field">
        <label htmlFor="price">Precio (ARS)</label>
        <input id="price" name="price" type="number" min="0" step="0.01" required />
      </div>

      {timesEntered && (
        <p className="hint">
          Duración del viaje: {formatDuration(minutesBetween(departureTime, arrivalTime))}
          {arrivesNextDay && ' — llega al día siguiente'}
        </p>
      )}

      <button type="submit" disabled={publishing}>
        {publishing ? 'Publicando…' : 'Publicar salida'}
      </button>

      {state?.ok === false && <p className="feedback feedback-error">{state.error}</p>}
      {state?.ok && <p className="feedback feedback-ok">Salida publicada.</p>}
    </form>
  )
}

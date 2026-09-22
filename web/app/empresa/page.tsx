import { requireRole } from '@/lib/auth'
import { formatDate, formatDuration, formatPrice, formatTime } from '@/lib/format'
import AppHeader from '../AppHeader'
import DeleteButton from './DeleteButton'
import PublishForm from './PublishForm'

export default async function CompanyPage() {
  const { supabase, email, company } = await requireRole('company')

  const [{ data: routes }, { data: departures }] = await Promise.all([
    supabase.from('routes').select('id, origin, destination').order('id'),
    supabase
      .from('schedules')
      .select('id, date, time, price, duration_minutes, route:routes (origin, destination)')
      .eq('company_id', company!.id)
      .order('date')
      .order('time'),
  ])

  return (
    <div className="company">
      <AppHeader badge={company!.name} email={email} />

      <main>
        <h1>Publicá una salida</h1>
        <p>
          Publicá tus horarios y precios para que los viajeros los encuentren
          junto a los del resto de las empresas que hacen la ruta.
        </p>

        <PublishForm routes={routes ?? []} />

        <h2>Salidas publicadas</h2>
        {!departures?.length ? (
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
                  <th><span className="visually-hidden">Acciones</span></th>
                </tr>
              </thead>
              <tbody>
                {departures.map(departure => {
                  const route = departure.route as unknown as { origin: string, destination: string }

                  return (
                    <tr key={departure.id}>
                      <td>{formatDate(departure.date)}</td>
                      <td>{route.origin} → {route.destination}</td>
                      <td>{formatTime(departure.time)}</td>
                      <td>{formatDuration(departure.duration_minutes)}</td>
                      <td>{formatPrice(departure.price)}</td>
                      <td>
                        <DeleteButton
                          id={departure.id}
                          label={`${route.origin} → ${route.destination} del ${formatDate(departure.date)} a las ${formatTime(departure.time)}`}
                        />
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
      </main>
    </div>
  )
}

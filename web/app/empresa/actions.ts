'use server'

import { revalidatePath } from 'next/cache'
import { requireRole } from '@/lib/auth'
import { minutesBetween } from '@/lib/format'

export type PublishState = { ok: true } | { ok: false, error: string } | null

export async function publishDeparture(_previous: PublishState, formData: FormData): Promise<PublishState> {
  const { supabase, company } = await requireRole('company')

  const departureTime = String(formData.get('departureTime'))
  const arrivalTime = String(formData.get('arrivalTime'))

  // The company comes from the signed-in account, never from the form, and
  // the database refuses any other one anyway.
  const { error } = await supabase.from('schedules').insert({
    company_id: company!.id,
    route_id: Number(formData.get('routeId')),
    date: String(formData.get('date')),
    time: departureTime,
    duration_minutes: minutesBetween(departureTime, arrivalTime),
    price: Number(formData.get('price')),
  })

  if (error?.code === '23505') {
    return { ok: false, error: 'Ya publicaste una salida en ese recorrido, ese día y a esa hora.' }
  }
  if (error) return { ok: false, error: 'No se pudo publicar la salida.' }

  revalidatePath('/empresa')
  return { ok: true }
}

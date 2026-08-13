const priceFormatter = new Intl.NumberFormat('es-AR', {
  style: 'currency',
  currency: 'ARS',
  maximumFractionDigits: 0,
})

export function formatPrice(price: number) {
  return priceFormatter.format(price)
}

// The API sends dates as 2026-08-20 and times as 06:00:00.
export function formatDate(date: string) {
  const [year, month, day] = date.split('-')
  return `${day}/${month}/${year}`
}

export function formatTime(time: string) {
  return time.slice(0, 5)
}

export function formatDuration(minutes: number) {
  const hours = Math.floor(minutes / 60)
  const rest = minutes % 60

  if (hours === 0) return `${rest} min`
  if (rest === 0) return `${hours} h`
  return `${hours} h ${rest} min`
}

// The form asks when the service departs and when it arrives, but the API
// stores how long it takes. An arrival that is not after the departure means
// the trip runs past midnight, so it lands on the next day.
export function minutesBetween(departureTime: string, arrivalTime: string) {
  const [departureHour, departureMinute] = departureTime.split(':').map(Number)
  const [arrivalHour, arrivalMinute] = arrivalTime.split(':').map(Number)

  const elapsed = (arrivalHour * 60 + arrivalMinute) - (departureHour * 60 + departureMinute)

  return elapsed > 0 ? elapsed : elapsed + 24 * 60
}

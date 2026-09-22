const priceFormatter = new Intl.NumberFormat('es-AR', {
  style: 'currency',
  currency: 'ARS',
  maximumFractionDigits: 0,
})

export function formatPrice(price: number) {
  return priceFormatter.format(price)
}

// Postgres sends dates as 2026-09-22.
export function formatDate(date: string) {
  const [year, month, day] = date.split('-')
  return `${day}/${month}/${year}`
}

// Postgres sends times as 06:00:00.
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

// An arrival at or before the departure time means the trip lands the next day.
export function minutesBetween(departureTime: string, arrivalTime: string) {
  const [departureHour, departureMinute] = departureTime.split(':').map(Number)
  const [arrivalHour, arrivalMinute] = arrivalTime.split(':').map(Number)

  const elapsed = (arrivalHour * 60 + arrivalMinute) - (departureHour * 60 + departureMinute)

  return elapsed > 0 ? elapsed : elapsed + 24 * 60
}

// The database stores how long a trip takes, not when it lands, so the arrival
// is worked out here. A long enough trip arrives the next day.
export function arrivalOf(departureTime: string, durationMinutes: number) {
  const [hour, minute] = departureTime.split(':').map(Number)
  const total = hour * 60 + minute + durationMinutes
  const minutesIntoDay = total % (24 * 60)

  const arrival = [Math.floor(minutesIntoDay / 60), minutesIntoDay % 60]
    .map(part => String(part).padStart(2, '0'))
    .join(':')

  return { arrival, daysLater: Math.floor(total / (24 * 60)) }
}

// Pages render on Vercel's servers, which run on UTC, so "today" has to be
// asked for in Argentina's time zone explicitly.
export function todayIsoDate() {
  return new Intl.DateTimeFormat('en-CA', { timeZone: 'America/Argentina/Buenos_Aires' }).format(new Date())
}

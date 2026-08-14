const priceFormatter = new Intl.NumberFormat('es-AR', {
  style: 'currency',
  currency: 'ARS',
  maximumFractionDigits: 0,
})

export function formatPrice(price: number) {
  return priceFormatter.format(price)
}

// The API sends times as 06:00:00.
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

// The API stores how long a trip takes, not when it lands, so the arrival is
// worked out here. A long enough trip arrives the next day.
export function arrivalOf(departureTime: string, durationMinutes: number) {
  const [hour, minute] = departureTime.split(':').map(Number)
  const total = hour * 60 + minute + durationMinutes
  const minutesIntoDay = total % (24 * 60)

  const arrival = [Math.floor(minutesIntoDay / 60), minutesIntoDay % 60]
    .map(part => String(part).padStart(2, '0'))
    .join(':')

  return { arrival, daysLater: Math.floor(total / (24 * 60)) }
}

export function todayIsoDate() {
  const now = new Date()
  const localMidnight = new Date(now.getTime() - now.getTimezoneOffset() * 60000)

  return localMidnight.toISOString().slice(0, 10)
}

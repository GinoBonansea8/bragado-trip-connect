'use client'

import { useTransition } from 'react'
import { deleteDeparture } from './actions'

export default function DeleteButton({ id, label }: { id: number, label: string }) {
  const [deleting, startTransition] = useTransition()

  function handleClick() {
    // Taking a departure down can't be undone, so it is confirmed first.
    if (!confirm(`¿Eliminar la salida ${label}?`)) return

    startTransition(async () => {
      try {
        await deleteDeparture(id)
      } catch {
        alert('No se pudo eliminar la salida.')
      }
    })
  }

  return (
    <button type="button" className="danger-button" onClick={handleClick} disabled={deleting}>
      {deleting ? 'Eliminando…' : 'Eliminar'}
    </button>
  )
}

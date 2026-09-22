'use client'

import { useActionState } from 'react'
import { signIn } from './actions'

export default function LoginForm() {
  const [error, action, pending] = useActionState(signIn, null)

  return (
    <form className="panel" action={action}>
      <div className="field">
        <label htmlFor="email">Email</label>
        <input id="email" name="email" type="email" autoComplete="email" required />
      </div>

      <div className="field">
        <label htmlFor="password">Contraseña</label>
        <input id="password" name="password" type="password" autoComplete="current-password" required />
      </div>

      <button type="submit" disabled={pending}>
        {pending ? 'Ingresando…' : 'Ingresar'}
      </button>

      {error && <p className="feedback feedback-error">{error}</p>}
    </form>
  )
}

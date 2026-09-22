import { signOut } from './actions'

interface Props {
  badge: string
  email: string
}

export default function AppHeader({ badge, email }: Props) {
  return (
    <header>
      <span className="brand">Bragado Trip Connect</span>
      <span className="badge">{badge}</span>

      <div className="header-end">
        <span>{email}</span>
        <form action={signOut}>
          <button type="submit" className="link-button">Salir</button>
        </form>
      </div>
    </header>
  )
}

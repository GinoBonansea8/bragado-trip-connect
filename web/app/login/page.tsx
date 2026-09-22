import { redirect } from 'next/navigation'
import { createClient } from '@/lib/supabase/server'
import LoginForm from './LoginForm'

export default async function LoginPage({ searchParams }: PageProps<'/login'>) {
  const { error } = await searchParams

  const supabase = await createClient()
  const { data: { user } } = await supabase.auth.getUser()

  // Already signed in: skip the form. Not for an account without a profile,
  // which the home page would only bounce back here.
  if (user && error !== 'profile') redirect('/')

  return (
    <>
      <header>
        <span className="brand">Bragado Trip Connect</span>
      </header>

      <main className="login">
        <h1>Ingresá</h1>
        <p>
          Las empresas publican sus salidas; los viajeros las comparan todas
          juntas.
        </p>

        <LoginForm />

        {error === 'profile' && (
          <p className="feedback feedback-error">
            Tu cuenta todavía no tiene un rol asignado.
          </p>
        )}

        <p className="demo">
          Cuentas de prueba, las dos con la contraseña <code>btp12345</code>:
          <br />
          <code>empresa@btp.com</code> para publicar salidas y
          <code> usuario@btp.com</code> para buscarlas.
        </p>
      </main>
    </>
  )
}

import { redirect } from 'next/navigation'
import { createClient } from './supabase/server'

export type Role = 'company' | 'traveler'

export const homeOf: Record<Role, string> = {
  company: '/empresa',
  traveler: '/viajes',
}

// Who is signed in and what they are allowed to do. Sends anyone without a
// session to the login page.
export async function currentProfile() {
  const supabase = await createClient()
  const { data: { user } } = await supabase.auth.getUser()

  if (!user) redirect('/login')

  const { data: profile } = await supabase
    .from('profiles')
    .select('role, company:companies (id, name)')
    .eq('user_id', user.id)
    .single()

  // An account with no profile can't do anything in the app.
  if (!profile) redirect('/login?error=profile')

  return {
    supabase,
    email: user.email ?? '',
    role: profile.role as Role,
    company: profile.company as unknown as { id: number, name: string } | null,
  }
}

// For pages only one kind of account may open: the other kind is sent to
// its own home instead.
export async function requireRole(role: Role) {
  const profile = await currentProfile()

  if (profile.role !== role) redirect(homeOf[profile.role])

  return profile
}

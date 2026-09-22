// Creates the two demo accounts and their profiles. Safe to run more than once.
// Needs NEXT_PUBLIC_SUPABASE_URL and SUPABASE_SERVICE_ROLE_KEY, which
// `vercel env pull .env.local` brings down once Supabase is connected.
//
//   node --env-file=.env.local scripts/seed-users.mjs

import { createClient } from '@supabase/supabase-js'

const DEMO_PASSWORD = 'btp12345'

const accounts = [
  { email: 'empresa@btp.com', role: 'company', companyCuit: '30-00000001-7' },
  { email: 'usuario@btp.com', role: 'traveler', companyCuit: null },
]

const supabase = createClient(
  process.env.NEXT_PUBLIC_SUPABASE_URL,
  process.env.SUPABASE_SERVICE_ROLE_KEY,
  { auth: { persistSession: false } },
)

async function findUserId(email) {
  const { data, error } = await supabase.auth.admin.listUsers({ perPage: 1000 })
  if (error) throw error

  return data.users.find(user => user.email === email)?.id
}

async function companyIdOf(cuit) {
  if (!cuit) return null

  const { data, error } = await supabase.from('companies').select('id').eq('cuit', cuit).single()
  if (error) throw error

  return data.id
}

for (const account of accounts) {
  let userId = await findUserId(account.email)

  if (!userId) {
    const { data, error } = await supabase.auth.admin.createUser({
      email: account.email,
      password: DEMO_PASSWORD,
      email_confirm: true,
    })
    if (error) throw error

    userId = data.user.id
  }

  const { error } = await supabase.from('profiles').upsert({
    user_id: userId,
    role: account.role,
    company_id: await companyIdOf(account.companyCuit),
  })
  if (error) throw error

  console.log(`${account.email} is ready (${account.role})`)
}

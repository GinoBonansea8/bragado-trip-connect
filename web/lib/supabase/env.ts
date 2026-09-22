// The Supabase integration on Vercel sets these. Newer projects name the
// public key "publishable" instead of "anon"; either one works here.
export const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL!

export const supabaseKey =
  process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY ?? process.env.NEXT_PUBLIC_SUPABASE_PUBLISHABLE_KEY!

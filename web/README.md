# Bragado Trip Connect — web

The deployed version of Bragado Trip Connect: a single Next.js app on Vercel,
backed by Supabase (Postgres + Auth). It lives next to the .NET backend and the
two React apps rather than replacing them — Vercel can't run .NET, so this is
the version that can be opened from a link.

**Live:** https://bragado-trip-connect.vercel.app

| Account           | Password   | Lands on                                   |
| ----------------- | ---------- | ------------------------------------------ |
| `empresa@btp.com` | `btp12345` | `/empresa` — publish departures for 21900  |
| `usuario@btp.com` | `btp12345` | `/viajes` — browse every operator's trips  |

## How it fits together

- **Auth** is Supabase Auth with email and password. Each user has a row in
  `profiles` saying whether they are an operator (`company`, tied to one
  company) or a `traveler`. After signing in, `/` sends each one to their view.
- **Pages are Server Components** that query Supabase as the signed-in user.
  Publishing a departure is a Server Action.
- **Permissions live in the database.** Row level security lets any signed-in
  user read departures, but only an operator can insert one, and only for its
  own company — even if someone calls the Supabase API directly.
- **`proxy.ts`** refreshes the Supabase session on every request so nobody is
  signed out after the one-hour token expires.

## Running it

The Supabase project was created from the Vercel Marketplace, which also set
its environment variables on the Vercel project. To work locally:

```bash
npx vercel link                 # once, to connect this folder to the project
npx vercel env pull .env.local  # brings down the Supabase keys
npm install
npm run dev                     # http://localhost:3000
```

## Setting up a fresh database

```bash
psql "$POSTGRES_URL_NON_POOLING" -f supabase/schema.sql   # tables, policies, seed data
node --env-file=.env.local scripts/seed-users.mjs         # the two demo accounts
```

Both are safe to run more than once.

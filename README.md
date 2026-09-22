# Bragado Trip Connect

A full-stack web application to compare intercity trip options between
**Bragado** and **Buenos Aires**, Argentina.

**Live demo:** https://bragado-trip-connect.vercel.app — sign in as
`empresa@btp.com` to publish departures or `usuario@btp.com` to browse them
(password `btp12345` for both). The demo is the Next.js + Supabase version in
[`web/`](web/README.md); Vercel can't run the .NET backend described below.

## The problem

I was born in Bragado, a small city in Buenos Aires province, and moved to
the city of Buenos Aires to go to university. Every time I head back — to
visit family, for the holidays, or just for a weekend — I have to work out
how to get there. I'm far from the only one: many students have left Bragado
to study in a bigger city, and we all face the same trip.

There are four main ways to travel between Bragado and Buenos Aires:

- **21900** (bus)
- **Santorini Turismo** (bus)
- **Chevallier** (bus)
- **Trenes Argentinos** (train)

Each one has its own departure times, arrival times, stops, prices and
booking channels, and there is no single place that brings them together.
The only way to compare them is by hand, one operator at a time.

It is worse than comparing four websites, because some operators can only be
reached over WhatsApp. Finding out what trips exist means messaging each one,
waiting for a reply, asking follow-up questions, comparing the answers and
only then booking — and until someone at the company writes back, you simply
don't know your options.

This project aims to fix that: one place to look up every trip between
Bragado and Buenos Aires without waiting on anyone to answer a message, and
to compare them side by side by schedule, price and trip duration.

## Current status

**Both apps now work end to end**, which makes the core of the project real:

- In the **company app**, an operator picks a route, a date, a departure and
  arrival time and a fare, and the departure is stored in PostgreSQL.
- In the **traveler app**, someone picks a route and a date and gets every
  operator's departures for it in one table — sortable by departure time, price
  or duration, with the cheapest and the quickest marked, and arrivals that fall
  past midnight shown as landing the next day.

The four operators and the routes they serve ship with the database, so there
are no screens to manage them.

Still missing: booking a seat, and real schedule and fare data for the four
operators — everything in the database today was entered by hand. There is no
login either; the company app asks which operator is publishing and takes the
answer at face value.

## Architecture

```
┌──────────────────┐
│  Traveler app    │ ─┐
│  (React, :3000)  │  │
└──────────────────┘  │     GraphQL      ┌──────────────────┐     EF Core      ┌────────────┐
                      ├────────────────► │   ASP.NET Core   │ ───────────────► │ PostgreSQL │
┌──────────────────┐  │  ◄────────────── │  + HotChocolate  │ ◄─────────────── │            │
│  Company app     │ ─┘                  └──────────────────┘                  └────────────┘
│  (React, :3001)  │
└──────────────────┘
```

- **Frontends**: two React + TypeScript apps built with Vite, both querying the
  same GraphQL API through Apollo Client. The **traveler app** is for people
  looking to travel; the **company app** is for the transport operators to
  publish their schedules and fares. They are separate applications because
  they serve different audiences and will diverge as features are added.
- **Backend**: ASP.NET Core (.NET 10) exposing a GraphQL API via
  [HotChocolate](https://chillicream.com/docs/hotchocolate).
- **Database**: PostgreSQL, accessed through Entity Framework Core (Npgsql
  provider).
- **Orchestration**: Docker Compose runs the three services together for
  local development.

## Getting started

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose

### Run locally

```bash
cp .env.example .env
docker compose up --build
```

- Traveler app: http://localhost:3000
- Company app: http://localhost:3001
- Backend GraphQL endpoint: http://localhost:5000/graphql

Both apps call a `health` GraphQL query, which the backend resolves by reading
a row from PostgreSQL through EF Core — confirming the whole chain (React →
GraphQL → EF Core → PostgreSQL) works end to end.

### Running services individually (without Docker)

- Backend: `cd backend/src/BragadoTripConnect.Api && dotnet run`
- Traveler app: `cd frontend/traveler && npm install && npm run dev`
- Company app: `cd frontend/company && npm install && npm run dev`

## Roadmap

- [ ] Model the domain: transport companies, routes, schedules, and prices.
- [ ] Load real data for 21900, Santorini Turismo, Chevallier, and Trenes
      Argentinos.
- [x] Build the trip comparison UI (filter by date, sort by price/duration).
- [ ] Add automated tests (backend and frontend).
- [ ] Deploy a public demo.

## License

This project is licensed under the [MIT License](LICENSE).

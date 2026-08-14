# Bragado Trip Connect

A full-stack web application to compare intercity trip options between
**Bragado** and **Buenos Aires**, Argentina.

## The problem

I live in Bragado, a small city in Buenos Aires province, and I travel back
and forth to the city of Buenos Aires regularly to attend university. There
is no direct, fast way to make that trip — I have to choose between four
different transport operators, each with its own website (or none at all),
schedule, and pricing:

- **21900** (bus)
- **Santorini Turismo** (bus)
- **Chevallier** (bus)
- **Trenes Argentinos** (train)

Each one has different departure times, arrival times, stops, ticket prices,
and booking channels. Comparing them today means checking four separate
sources by hand every single time, with no way to see all the options side
by side. This project aims to fix that: a single place to look up and
compare every available trip between Bragado and Buenos Aires, across all
four operators.

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

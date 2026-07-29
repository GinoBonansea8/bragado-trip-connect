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

This repository currently contains the **technology foundation** for the
project: a working, containerized full-stack setup with a frontend, a
backend exposing a GraphQL API, and a PostgreSQL database wired together
end to end. Real schedule and pricing data for the four operators, along
with the actual trip-comparison features, will be added in upcoming
iterations.

## Architecture

```
┌─────────────┐        GraphQL        ┌──────────────────┐        EF Core       ┌────────────┐
│   React     │  ───────────────────► │   ASP.NET Core    │  ─────────────────► │ PostgreSQL │
│  (frontend) │  ◄─────────────────── │  + HotChocolate   │  ◄───────────────── │  (database) │
└─────────────┘                       └──────────────────┘                      └────────────┘
```

- **Frontend**: React + TypeScript, built with Vite, using Apollo Client to
  query the GraphQL API.
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

- Frontend: http://localhost:3000
- Backend GraphQL endpoint: http://localhost:5000/graphql

The frontend calls a `health` GraphQL query, which the backend resolves by
reading a row from PostgreSQL through EF Core — confirming the whole chain
(React → GraphQL → EF Core → PostgreSQL) works end to end.

### Running services individually (without Docker)

- Backend: `cd backend/src/BragadoTripConnect.Api && dotnet run`
- Frontend: `cd frontend && npm install && npm run dev`

## Roadmap

- [ ] Model the domain: transport companies, routes, schedules, and prices.
- [ ] Load real data for 21900, Santorini Turismo, Chevallier, and Trenes
      Argentinos.
- [ ] Build the trip comparison UI (filter by date, sort by price/duration).
- [ ] Add automated tests (backend and frontend).
- [ ] Deploy a public demo.

## License

This project is licensed under the [MIT License](LICENSE).

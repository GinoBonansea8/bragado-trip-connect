# Roadmap

Ideas and future directions for this project that go beyond the current
technology foundation. Also tracked as GitHub Issues once the repo is
public — this file is the running notebook version.

## Two separate frontends

Longer term, the plan is to split the single frontend into two applications
that share the same GraphQL backend:

1. **Operator-facing app**: used by the transport companies (e.g. 21900,
   Santorini Turismo, Chevallier, Trenes Argentinos) to manage and publish
   their own schedules, prices, and seat availability.
2. **Traveler-facing app**: used by people looking to travel between
   Bragado and Buenos Aires, to search and compare trips across all
   operators.

This isn't scoped yet (no auth model, no per-operator data ownership rules
decided) — it's a direction, not a committed design.

## Monetization idea: premium subscriptions

Tickets for high-demand dates (e.g. Christmas, New Year's) sell out fast, and
operators only open sales for those dates at specific announced times — you
can't buy them whenever you want. Idea: a premium subscription tier that
notifies subscribers the moment sales open for these special dates, giving
them a head start over non-subscribers.

Not scoped yet either — depends on the traveler-facing app existing first,
and on being able to detect/track when each operator opens sales for a given
date (may require polling or scraping operator sites, since none of them are
expected to offer a webhook/notification API).

## Near-term (see README)

- [ ] Model the domain: transport companies, routes, schedules, and prices.
- [ ] Load real data for the four operators.
- [ ] Build the trip comparison UI.
- [ ] Automated tests.
- [ ] Public demo deployment.

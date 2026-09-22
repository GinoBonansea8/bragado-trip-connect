-- Schema for the Vercel + Supabase version of Bragado Trip Connect.
-- It mirrors the tables of the .NET backend, plus a profile that tells an
-- operator account apart from a traveller one. Safe to run more than once.

create table if not exists public.companies (
  id bigint generated always as identity primary key,
  cuit text not null unique,
  name text not null
);

create table if not exists public.routes (
  id bigint generated always as identity primary key,
  origin text not null,
  destination text not null,
  unique (origin, destination)
);

-- One row per Supabase Auth user. Operator accounts publish on behalf of a
-- single company; traveller accounts have none.
create table if not exists public.profiles (
  user_id uuid primary key references auth.users on delete cascade,
  role text not null check (role in ('company', 'traveler')),
  company_id bigint references public.companies,
  check ((role = 'company') = (company_id is not null))
);

create table if not exists public.schedules (
  id bigint generated always as identity primary key,
  route_id bigint not null references public.routes,
  company_id bigint not null references public.companies,
  date date not null,
  time time not null,
  price numeric(12, 2) not null check (price >= 0),
  duration_minutes integer not null check (duration_minutes > 0),
  unique (company_id, route_id, date, time)
);

create index if not exists schedules_route_date on public.schedules (route_id, date);

-- The company the signed-in user publishes for, or null for travellers.
-- security definer lets the policies below read profiles without tripping
-- over the profiles policy itself.
create or replace function public.my_company_id()
returns bigint
language sql
stable
security definer
set search_path = ''
as $$
  select company_id from public.profiles where user_id = auth.uid()
$$;

-- Newer Supabase projects no longer grant the API roles access to new tables
-- on their own. What each role may actually see is narrowed down by the row
-- level security policies below.
grant select on public.companies, public.routes, public.profiles to authenticated;
grant select, insert on public.schedules to authenticated;
grant execute on function public.my_company_id() to authenticated;
grant all on public.companies, public.routes, public.profiles, public.schedules to service_role;

alter table public.companies enable row level security;
alter table public.routes enable row level security;
alter table public.profiles enable row level security;
alter table public.schedules enable row level security;

-- Anyone signed in can read the catalogue and every published departure.
drop policy if exists "Signed-in users read companies" on public.companies;
create policy "Signed-in users read companies" on public.companies
  for select to authenticated using (true);

drop policy if exists "Signed-in users read routes" on public.routes;
create policy "Signed-in users read routes" on public.routes
  for select to authenticated using (true);

drop policy if exists "Signed-in users read schedules" on public.schedules;
create policy "Signed-in users read schedules" on public.schedules
  for select to authenticated using (true);

drop policy if exists "Users read their own profile" on public.profiles;
create policy "Users read their own profile" on public.profiles
  for select to authenticated using (user_id = auth.uid());

-- Only an operator account can publish, and only for its own company.
drop policy if exists "Operators publish for their company" on public.schedules;
create policy "Operators publish for their company" on public.schedules
  for insert to authenticated with check (company_id = public.my_company_id());

insert into public.companies (cuit, name) values
  ('30-00000001-7', '21900'),
  ('30-00000002-5', 'Santorini Turismo'),
  ('30-00000003-3', 'Chevallier'),
  ('30-00000004-1', 'Trenes Argentinos')
on conflict (cuit) do nothing;

insert into public.routes (origin, destination) values
  ('Bragado', 'Once'),
  ('Bragado', 'Retiro'),
  ('Once', 'Bragado'),
  ('Retiro', 'Bragado')
on conflict (origin, destination) do nothing;

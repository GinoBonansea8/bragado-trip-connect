import ApiStatus from './ApiStatus'

const STOPS = ['Bragado', 'Retiro', 'Once']

function App() {
  return (
    <>
      <header>
        <span className="brand">Bragado Trip Connect</span>
        <span className="badge">Travellers</span>
      </header>

      <main>
        <h1>Find your trip</h1>
        <p>
          Every departure between Bragado and Buenos Aires — 21900, Santorini
          Turismo, Chevallier and Trenes Argentinos — in one place, side by side.
        </p>

        <form className="panel">
          <div className="row">
            <div className="field">
              <label htmlFor="origin">From</label>
              <select id="origin" defaultValue="Bragado" disabled>
                {STOPS.map(stop => <option key={stop}>{stop}</option>)}
              </select>
            </div>

            <div className="field">
              <label htmlFor="destination">To</label>
              <select id="destination" defaultValue="Retiro" disabled>
                {STOPS.map(stop => <option key={stop}>{stop}</option>)}
              </select>
            </div>

            <div className="field">
              <label htmlFor="date">Date</label>
              <input id="date" type="date" disabled />
            </div>
          </div>

          <button type="submit" disabled>Search departures</button>
        </form>

        <h2>Departures</h2>
        <p className="placeholder">
          Results will land here once the API can answer a search.
        </p>

        <h2>My bookings</h2>
        <p className="placeholder">
          Trips you reserve will be listed here.
        </p>

        <ApiStatus />
      </main>
    </>
  )
}

export default App

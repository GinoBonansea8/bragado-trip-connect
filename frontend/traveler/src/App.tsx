import ApiStatus from './ApiStatus'

const STOPS = ['Bragado', 'Retiro', 'Once']

function App() {
  return (
    <>
      <header>
        <span className="brand">Bragado Trip Connect</span>
        <span className="badge">Viajeros</span>
      </header>

      <main>
        <h1>Encontrá tu viaje</h1>
        <p>
          Todas las salidas entre Bragado y Buenos Aires — 21900, Santorini
          Turismo, Chevallier y Trenes Argentinos — en un solo lugar, una al
          lado de la otra.
        </p>

        <form className="panel">
          <div className="row">
            <div className="field">
              <label htmlFor="origin">Desde</label>
              <select id="origin" defaultValue="Bragado" disabled>
                {STOPS.map(stop => <option key={stop}>{stop}</option>)}
              </select>
            </div>

            <div className="field">
              <label htmlFor="destination">Hasta</label>
              <select id="destination" defaultValue="Retiro" disabled>
                {STOPS.map(stop => <option key={stop}>{stop}</option>)}
              </select>
            </div>

            <div className="field">
              <label htmlFor="date">Fecha</label>
              <input id="date" type="date" disabled />
            </div>
          </div>

          <button type="submit" disabled>Buscar salidas</button>
        </form>

        <h2>Salidas</h2>
        <p className="placeholder">
          Acá van a aparecer los resultados, cuando la API pueda responder una
          búsqueda.
        </p>

        <h2>Mis reservas</h2>
        <p className="placeholder">
          Los viajes que reserves se van a listar acá.
        </p>

        <ApiStatus />
      </main>
    </>
  )
}

export default App

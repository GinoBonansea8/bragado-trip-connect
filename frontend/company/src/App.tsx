import ApiStatus from './ApiStatus'

const COMPANIES = ['21900', 'Santorini Turismo', 'Chevallier', 'Trenes Argentinos']
const STOPS = ['Bragado', 'Retiro', 'Once']

function App() {
  return (
    <>
      <header>
        <span className="brand">Bragado Trip Connect</span>
        <span className="badge">Empresas</span>
      </header>

      <main>
        <h1>Publicá una salida</h1>
        <p>
          Publicá tus horarios y precios para que los viajeros los encuentren
          junto a los del resto de las empresas que hacen la ruta.
        </p>

        <form className="panel">
          <div className="field">
            <label htmlFor="company">Empresa</label>
            <select id="company" disabled>
              {COMPANIES.map(company => <option key={company}>{company}</option>)}
            </select>
          </div>

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
          </div>

          <div className="row">
            <div className="field">
              <label htmlFor="date">Fecha</label>
              <input id="date" type="date" disabled />
            </div>

            <div className="field">
              <label htmlFor="departure">Sale</label>
              <input id="departure" type="time" disabled />
            </div>

            <div className="field">
              <label htmlFor="arrival">Llega (aprox.)</label>
              <input id="arrival" type="time" disabled />
            </div>
          </div>

          <div className="field">
            <label htmlFor="price">Precio (ARS)</label>
            <input id="price" type="number" min="0" step="0.01" disabled />
          </div>

          <button type="submit" disabled>Publicar salida</button>
        </form>

        <h2>Salidas publicadas</h2>
        <p className="placeholder">
          Las salidas que publiques se van a listar acá.
        </p>

        <ApiStatus />
      </main>
    </>
  )
}

export default App

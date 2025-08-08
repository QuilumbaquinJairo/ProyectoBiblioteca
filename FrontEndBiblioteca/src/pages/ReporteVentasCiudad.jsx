import React, { useEffect, useState } from 'react';
import axios from '../api/axiosInstance';

export default function ReportesVentasPorCiudad() {
  const [ciudades, setCiudades] = useState([]);
  const [resultados, setResultados] = useState([]);
  const [fechaInicio, setFechaInicio] = useState('');
  const [fechaFin, setFechaFin] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    axios.get('https://localhost:7172/ciudades')
      .then(res => setCiudades(res.data))
      .catch(err => console.error(err));
  }, []);

  const buscar = async () => {
    if (!fechaInicio || !fechaFin) return alert('Seleccione ambas fechas');

    setLoading(true);
    try {
      const res = await axios.get(`https://localhost:7172/reportes/ventas-por-ciudad`, {
        params: { fechaInicio, fechaFin }
      });
      setResultados(res.data);
    } catch (err) {
      console.error(err);
      alert('Error al obtener datos');
    }
    setLoading(false);
  };

  const imprimir = () => {
    window.print();
  };

  return (
    <div className="container mt-4">
      <h2 className="mb-4">📊 Reporte de Ventas por Ciudad</h2>

      <div className="row g-3 align-items-end mb-3">
        <div className="col-md-4">
          <label className="form-label">Fecha Inicio</label>
          <input type="date" className="form-control" value={fechaInicio} onChange={e => setFechaInicio(e.target.value)} />
        </div>
        <div className="col-md-4">
          <label className="form-label">Fecha Fin</label>
          <input type="date" className="form-control" value={fechaFin} onChange={e => setFechaFin(e.target.value)} />
        </div>
        <div className="col-md-4 d-flex gap-2">
          <button className="btn btn-primary" onClick={buscar} disabled={loading}>
            {loading ? 'Cargando...' : 'Buscar'}
          </button>
          <button className="btn btn-outline-secondary" onClick={imprimir}>Imprimir</button>
        </div>
      </div>

      <table className="table table-bordered mt-4">
        <thead className="table-light">
          <tr>
            <th>Ciudad</th>
            <th>Total Vendido (USD)</th>
          </tr>
        </thead>
        <tbody>
          {resultados.map((r, index) => (
            <tr key={index}>
              <td>{r.ciudad}</td>
              <td>${r.totalVendido.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

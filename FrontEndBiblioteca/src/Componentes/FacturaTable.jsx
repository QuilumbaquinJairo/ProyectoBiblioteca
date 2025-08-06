export default function FacturaTable({ facturas, onEdit, onView, onDelete }) {
  return (
    <table className="table">
      <thead>
        <tr>
          <th>ID</th><th>Fecha</th><th>RUC Cliente</th><th>Ciudad</th><th>Total</th><th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        {facturas.map(f => (
          <tr key={f.numeroFactura}>
            <td>{f.numeroFactura}</td>
            <td>{new Date(f.fecha).toLocaleDateString()}</td>
            <td>{f.cliente}</td>
            <td>{f.ciudadEntrega}</td>
            <td>${f.total.toFixed(2)}</td>
            <td>
              <button className="btn btn-sm btn-link" onClick={() => onView(f.numeroFactura)}>Ver</button> |
              <button className="btn btn-sm btn-link" onClick={() => onEdit(f.numeroFactura)}>Editar</button> |
              <button className="btn btn-sm btn-link text-danger" onClick={() => onDelete(f.numeroFactura)}>Eliminar</button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

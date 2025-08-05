import React from 'react';

export default function ArticuloTable({ articulos, onEdit, onDelete }) {
  return (
    <table className="table table-bordered">
      <thead className="table-light">
        <tr>
          <th>Code</th>
          <th>Name</th>
          <th>Inventory</th>
          <th>Price</th>
          <th className="text-center">Actions</th>
        </tr>
      </thead>
      <tbody>
        {articulos.map((a) => (
          <tr key={a.codigoArticulo}>
            <td>{a.codigoArticulo}</td>
            <td>{a.nombreArticulo}</td>
            <td>{a.saldoInventario}</td>
            <td>${a.precio.toFixed(2)}</td>
            <td className="text-center">
              <button className="btn btn-link" onClick={() => onEdit(a)}>Edit</button> |
              <button className="btn btn-link text-danger" onClick={() => onDelete(a.codigoArticulo)}>Delete</button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

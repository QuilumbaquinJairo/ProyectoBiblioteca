// src/Componentes/ClienteTable.jsx
import React from 'react';

export default function ClienteTable({ clientes, onEdit, onDelete }) {
  return (
    <div className="overflow-x-auto">
      <table className="min-w-full border border-gray-300 rounded-md">
        <thead className="bg-gray-100">
          <tr>
            <th className="px-4 py-2 text-left text-sm">RUC</th>
            <th className="px-4 py-2 text-left text-sm">Nombre</th>
            <th className="px-4 py-2 text-left text-sm">Dirección</th>
            <th className="px-4 py-2 text-center text-sm">Acciones</th>
          </tr>
        </thead>
        <tbody>
          {clientes.map((cliente) => (
            <tr key={cliente.ruc} className="border-t text-sm">
              <td className="px-4 py-2">{cliente.ruc}</td>
              <td className="px-4 py-2">{cliente.nombre}</td>
              <td className="px-4 py-2">{cliente.direccion}</td>
              <td className="px-4 py-2 text-center space-x-2">
                <button
                  type="button"
                  onClick={() => onEdit(cliente)}
                  className="text-blue-600 hover:underline"
                >
                  Editar
                </button>
                <button
                  type="button"
                  onClick={() => onDelete(cliente.ruc)}
                  className="text-red-600 hover:underline"
                >
                  Eliminar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

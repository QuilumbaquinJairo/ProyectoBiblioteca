// src/components/ClienteForm.jsx
import React, { useState } from 'react';
import Cookies from 'js-cookie';
export default function ClienteForm({ onClose, onSuccess, clienteEdit = null }) {
  const [ruc, setRuc] = useState(clienteEdit?.ruc || '');
  const [nombre, setNombre] = useState(clienteEdit?.nombre || '');
  const [direccion, setDireccion] = useState(clienteEdit?.direccion || '');
  const [error, setError] = useState('');

  const isEdit = Boolean(clienteEdit);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const url = isEdit
        ? `https://localhost:7172/clientes/${clienteEdit.ruc}`
        : 'https://localhost:7172/clientes';

      const method = isEdit ? 'PUT' : 'POST';

      const payload = isEdit
        ? { nombre, direccion }
        : { ruc, nombre, direccion };

      const res = await fetch(url, {
        method,
        headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${Cookies.get('auth_token')}`
        },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const data = await res.json();
        throw new Error(data.message || 'Error en la operación');
      }

      onSuccess();
      onClose();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-md w-full max-w-md shadow-lg">
        <h2 className="text-xl font-semibold mb-4">
          {isEdit ? 'Editar Cliente' : 'Registrar Cliente'}
        </h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          {!isEdit && (
            <div>
              <label className="block text-sm font-medium text-gray-700">RUC</label>
              <input
                type="text"
                value={ruc}
                onChange={(e) => setRuc(e.target.value)}
                required
                className="w-full mt-1 px-4 py-2 border border-gray-300 rounded-md"
              />
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700">Nombre</label>
            <input
              type="text"
              value={nombre}
              onChange={(e) => setNombre(e.target.value)}
              required
              className="w-full mt-1 px-4 py-2 border border-gray-300 rounded-md"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">Dirección</label>
            <input
              type="text"
              value={direccion}
              onChange={(e) => setDireccion(e.target.value)}
              required
              className="w-full mt-1 px-4 py-2 border border-gray-300 rounded-md"
            />
          </div>

          {error && <p className="text-red-500 text-sm">{error}</p>}

          <div className="flex justify-end space-x-2">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 bg-gray-200 rounded-md"
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition"
            >
              {isEdit ? 'Actualizar' : 'Registrar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

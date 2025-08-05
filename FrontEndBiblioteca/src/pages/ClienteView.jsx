// src/pages/ClienteView.jsx
import React, { useState, useEffect } from 'react';
import ClienteTable from '../Componentes/ClienteTable';
import ClienteForm from '../Componentes/ClienteForm';
import SearchBar from '../Componentes/SearchBar';
import Cookies from 'js-cookie';

export default function ClienteView() {
  const [clientes, setClientes] = useState([]);
  const [busqueda, setBusqueda] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editCliente, setEditCliente] = useState(null);

  const fetchClientes = async () => {
    const res = await fetch('https://localhost:7172/clientes', {
      headers: { Authorization: `Bearer ${Cookies.get('auth_token')}` },
    });

    if (!res.ok) throw new Error('Error al obtener clientes');

    const text = await res.text();
    if (!text) return;

    setClientes(JSON.parse(text));
  };

  const handleEdit = (cliente) => {
    setEditCliente(cliente);
    setShowForm(true);
  };

  const handleDelete = async (ruc) => {
    if (!window.confirm('¿Deseas eliminar este cliente?')) return;

    await fetch(`https://localhost:7172/clientes/${ruc}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${Cookies.get('auth_token')}` },
    });

    fetchClientes();
  };

  const handleBuscar = (valor) => setBusqueda(valor);

  const handleCloseForm = () => {
    setShowForm(false);
    setEditCliente(null);
  };

  useEffect(() => {
    fetchClientes();
  }, []);

  return (
    <div className="container mt-4">
      <div className="mb-4">
        <h1 className="h3">Gestión de Clientes</h1>
        <p className="text-muted">
          Administra la información de tus clientes, realiza búsquedas, actualizaciones y eliminaciones.
        </p>
      </div>

      <div className="mb-3">
        <SearchBar onSearch={handleBuscar} />
      </div>

      <ClienteTable
        clientes={clientes}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />

      <ClienteForm
        show={showForm}
        onClose={() => setShowForm(false)}
        onSuccess={fetchClientes}
        clienteEdit={editCliente}
      />


      <div className="mt-4">
        <button
          className="btn btn-primary"
          onClick={() => setShowForm(true)}
        >
          Registrar nuevo cliente
        </button>
      </div>
    </div>
  );
}

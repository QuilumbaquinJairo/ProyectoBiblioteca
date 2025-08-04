// src/pages/ClienteView.jsx
import React, { useState, useEffect } from 'react';
import Navbar from '../Componentes/Navbar';
import ClienteTable from '../Componentes/ClienteTable';
import ClienteForm from '../Componentes/ClienteForm';
import SearchBar from '../Componentes/SearchBar';
import Cookies from 'js-cookie';

export default function ClienteView() {
  const [clientes, setClientes] = useState([]);
  const [busqueda, setBusqueda] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editCliente, setEditCliente] = useState(null);
  const handleEdit = (cliente) => {
        setEditCliente(cliente);
        setShowForm(true);
 };

    const fetchClientes = async () => {
        const token = Cookies.get('auth_token');

        const res = await fetch('https://localhost:7172/clientes', {
            headers: {
            Authorization: `Bearer ${token}`
            }
        });

        if (!res.ok) throw new Error('Error al obtener clientes');

        const text = await res.text();
        if (!text) return;

        const data = JSON.parse(text);
        setClientes(data);
    };


    const handleDelete = async (ruc) => {
        if (!window.confirm('¿Deseas eliminar este cliente?')) return;
        await fetch(`https://localhost:7172/clientes/${ruc}`, 
            { method: 'DELETE'
            , headers: { Authorization: `Bearer ${Cookies.get('auth_token')}` }
            });
        fetchClientes();
    };

  useEffect(() => {
    fetchClientes();
  }, []);

  const handleBuscar = (valor) => {
    setBusqueda(valor);
    // (Opcional) aplicar filtro por búsqueda local o remoto
  };

  return (
    <div>
      <Navbar />
      <main className="p-6">
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-gray-800">Gestión de Clientes</h1>
          <p className="text-sm text-gray-500">Administra la información de tus clientes, realiza búsquedas, actualizaciones y eliminaciones.</p>
        </div>

        <div className="mb-4">
          <SearchBar onSearch={handleBuscar} />
        </div>

        <ClienteTable clientes={clientes} onEdit={handleEdit} onDelete={handleDelete} />
        {showForm && (
        <ClienteForm
            onClose={() => {
            setShowForm(false);
            setEditCliente(null);
            }}
            onSuccess={fetchClientes}
            clienteEdit={editCliente}
        />
        )}


        <div className="mt-6">
          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600 transition"
          >
            Registrar nuevo cliente
          </button>
        </div>
      </main>
    </div>
  );
}

import React, { useState, useEffect } from 'react';
import FacturaTable from '../Componentes/FacturaTable';
import FacturaFormModal from '../Componentes/FacturaFormModal';
import FacturaViewModal from '../Componentes/FacturaViewModal';
import { authHeaders } from '../utils/auth';

export default function FacturasView() {
  const [facturas, setFacturas] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState(null);
  const [viewId, setViewId] = useState(null);

  const fetchFacturas = async () => {
    const res = await fetch('https://localhost:7172/facturas', { headers: authHeaders() });
    if (!res.ok) throw new Error('Error');
    setFacturas(await res.json());
  };
  useEffect(() => { fetchFacturas(); }, []);

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between mb-3">
        <h2>Gestión de Facturas</h2>
        <button className="btn btn-outline-primary" onClick={() => setShowForm(true)}>
          Nueva Factura
        </button>
      </div>
      <FacturaTable
        facturas={facturas}
        onEdit={(id) => setEditId(id)}
        onView={(id) => setViewId(id)}
        onDelete={async (id) => {
          if (window.confirm('Eliminar?')) {
            await fetch(`https://localhost:7172/facturas/${id}`, { method: 'DELETE', headers: authHeaders() });
            fetchFacturas();
          }
        }}
      />

      <FacturaFormModal
        show={showForm || !!editId}
        onClose={() => { setShowForm(false); setEditId(null); }}
        onSuccess={fetchFacturas}
        facturaId={editId}
      />

      <FacturaViewModal
        show={!!viewId}
        onClose={() => setViewId(null)}
        facturaId={viewId}
      />
    </div>
  );
}

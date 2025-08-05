import React, { useEffect, useState } from 'react';
import ArticuloTable from '../Componentes/ArticuloTable';
import ArticuloFormModal from '../Componentes/ArticuloFormModal';
import DeleteConfirmModal from '../Componentes/DeleteConfirmModal';
import 'bootstrap/dist/css/bootstrap.min.css';
import { authHeaders } from '../utils/auth'; // Assuming you have a function to get auth headers

export default function ArticulosView() {
  const [articulos, setArticulos] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [editArticulo, setEditArticulo] = useState(null);
  const [showDelete, setShowDelete] = useState(false);
  const [deleteCode, setDeleteCode] = useState(null);

  const fetchArticulos = async () => {
    const res = await fetch('https://localhost:7172/articulos', {
        headers: authHeaders(),
    });
    const data = await res.json();
    setArticulos(data);
  };

  useEffect(() => {
    fetchArticulos();
  }, []);

  const handleEdit = (articulo) => {
    setEditArticulo(articulo);
    setShowForm(true);
  };

  const handleDelete = (codigo) => {
    setDeleteCode(codigo);
    setShowDelete(true);
  };

  const confirmDelete = async () => {
    await fetch(`https://localhost:7172/articulos/${deleteCode}`, { method: 'DELETE' });
    setShowDelete(false);
    fetchArticulos();
  };

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Articles</h2>
        <button className="btn btn-outline-primary" onClick={() => setShowForm(true)}>New Article</button>
      </div>

      <ArticuloTable
        articulos={articulos}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />

      {showForm && (
        <ArticuloFormModal
          articulo={editArticulo}
          onClose={() => {
            setShowForm(false);
            setEditArticulo(null);
          }}
          onSuccess={fetchArticulos}
        />
      )}

      {showDelete && (
        <DeleteConfirmModal
          onCancel={() => setShowDelete(false)}
          onConfirm={confirmDelete}
        />
      )}
    </div>
  );
}

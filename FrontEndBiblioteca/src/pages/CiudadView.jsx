// src/pages/CiudadView.jsx
import React, { useEffect, useState } from 'react';
import { Modal, Button, Form, Table, Alert } from 'react-bootstrap';
import { authHeaders } from '../utils/auth';

export default function CiudadView() {
  const [ciudades, setCiudades] = useState([]);
  const [nombreCiudad, setNombreCiudad] = useState('');
  const [ciudadEditando, setCiudadEditando] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [ciudadEliminar, setCiudadEliminar] = useState(null);
  const [mensajeExito, setMensajeExito] = useState('');
  const [error, setError] = useState('');

  const fetchCiudades = async () => {
    const res = await fetch('https://localhost:7172/ciudades', {
      headers: authHeaders(),
    });
    const data = await res.json();
    setCiudades(data);
  };

  useEffect(() => {
    fetchCiudades();
  }, []);

  const abrirModal = (ciudad = null) => {
    setCiudadEditando(ciudad);
    setNombreCiudad(ciudad ? ciudad.nombreCiudad : '');
    setShowModal(true);
    setError('');
  };

  const handleGuardar = async () => {
    if (!nombreCiudad.trim()) {
      setError('El nombre de la ciudad es obligatorio.');
      return;
    }

    const isDuplicado = ciudades.some(
      c => c.nombreCiudad.toLowerCase() === nombreCiudad.toLowerCase() &&
           (!ciudadEditando || c.codigoCiudad !== ciudadEditando.codigoCiudad)
    );
    if (isDuplicado) {
      setError('Este nombre ya existe.');
      return;
    }

    const method = ciudadEditando ? 'PUT' : 'POST';
    const url = ciudadEditando
      ? `https://localhost:7172/ciudades/${ciudadEditando.codigoCiudad}`
      : 'https://localhost:7172/ciudades';

    const res = await fetch(url, {
      method,
      headers: {
        ...authHeaders(),
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ nombreCiudad }),
    });

    if (!res.ok) {
      const msg = await res.text();
      setError(msg);
      return;
    }

    setShowModal(false);
    setMensajeExito(ciudadEditando ? 'Ciudad actualizada' : 'Ciudad registrada');
    fetchCiudades();
  };

  const handleEliminar = async () => {
    const res = await fetch(`https://localhost:7172/ciudades/${ciudadEliminar.codigoCiudad}`, {
      method: 'DELETE',
      headers: authHeaders(),
    });

    if (!res.ok) {
      setError('Error al eliminar');
      return;
    }

    setShowDeleteModal(false);
    fetchCiudades();
  };

  return (
    <div className="container mt-4">
      <h2>Ciudades Registradas</h2>
      {mensajeExito && <Alert variant="success">{mensajeExito}</Alert>}

      <div className="mb-3 text-end">
        <Button variant="primary" onClick={() => abrirModal()}>Agregar Ciudad</Button>
      </div>

      <Table bordered hover>
        <thead>
          <tr>
            <th>City ID</th>
            <th>City Name</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {ciudades.map(c => (
            <tr key={c.codigoCiudad}>
              <td>{c.codigoCiudad}</td>
              <td>{c.nombreCiudad}</td>
              <td>
                <Button variant="link" onClick={() => abrirModal(c)}>Editar</Button> |
                <Button variant="link" className="text-danger" onClick={() => {
                  setCiudadEliminar(c);
                  setShowDeleteModal(true);
                }}>Eliminar</Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      {/* Modal Crear / Editar */}
      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>{ciudadEditando ? 'Editar Ciudad' : 'Agregar Ciudad'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form.Group>
            <Form.Label>Nombre Ciudad</Form.Label>
            <Form.Control
              value={nombreCiudad}
              onChange={e => setNombreCiudad(e.target.value)}
              placeholder="Ingrese nombre"
              required
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>Cancelar</Button>
          <Button variant="primary" onClick={handleGuardar}>
            {ciudadEditando ? 'Actualizar' : 'Guardar'}
          </Button>
        </Modal.Footer>
      </Modal>

      {/* Modal Eliminar */}
      <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Eliminar Ciudad</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          ¿Estás seguro de que deseas eliminar la ciudad <strong>{ciudadEliminar?.nombreCiudad}</strong>?
          Esta acción no se puede deshacer.
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancelar</Button>
          <Button variant="danger" onClick={handleEliminar}>Confirmar</Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}

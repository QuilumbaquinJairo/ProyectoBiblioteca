// src/components/FacturaFormModal.jsx
import React, { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { authHeaders } from '../utils/auth';

export default function FacturaFormModal({ show, onClose, onSuccess, facturaId = null }) {
  const [fecha, setFecha] = useState('');
  const [rucCliente, setRucCliente] = useState('');
  const [ciudad, setCiudad] = useState('');
  const [detalles, setDetalles] = useState([{ codigoArticulo: '', cantidad: 1, precio: 0 }]);
  const [articulos, setArticulos] = useState([]);
  const [ciudades, setCiudades] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    fetch('https://localhost:7172/articulos', { headers: authHeaders() })
      .then(r => r.json()).then(setArticulos);
    fetch('https://localhost:7172/ciudades', { headers: authHeaders() })
      .then(r => r.json()).then(setCiudades);

    if (facturaId) {
      fetch(`https://localhost:7172/facturas/${facturaId}`, { headers: authHeaders() })
        .then(r => r.json())
        .then(f => {
          setFecha(f.fecha.slice(0, 10));
          setRucCliente(f.rucCliente);
          setCiudad(f.codigoCiudadEntrega);
          setDetalles(f.detalles);
        });
    }
  }, [facturaId]);

  const handleDetalleChange = (idx, field, value) => {
    const nuevos = [...detalles];
    nuevos[idx][field] = field === 'codigoArticulo' ? Number(value) : value;
    if (field === 'codigoArticulo') {
      const art = articulos.find(a => a.codigoArticulo === Number(value));
      nuevos[idx].precio = art?.precio || 0;
    }
    setDetalles(nuevos);
  };

  const total = detalles.reduce((sum, d) => sum + d.cantidad * d.precio, 0);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const payload = {
      fecha,
      codigoCiudadEntrega: Number(ciudad),
      rucCliente,
      detalles: detalles.map(d => ({
        codigoArticulo: Number(d.codigoArticulo),
        cantidad: Number(d.cantidad),
        precio: Number(d.precio),
      })),
    };

    const method = facturaId ? 'PUT' : 'POST';
    const url = facturaId
      ? `https://localhost:7172/facturas/${facturaId}`
      : 'https://localhost:7172/facturas';

    const res = await fetch(url, {
      method,
      headers: {
        ...authHeaders(),
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (!res.ok) {
      const data = await res.json();
      setError(data.message || 'Error en la operación');
      return;
    }

    onSuccess();
    onClose();
  };

  return (
    <Modal show={show} onHide={onClose} size="lg">
      <Modal.Header closeButton>
        <Modal.Title>{facturaId ? 'Editar Factura' : 'Nueva Factura'}</Modal.Title>
      </Modal.Header>
      <Form onSubmit={handleSubmit}>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Fecha</Form.Label>
            <Form.Control
              type="date"
              value={fecha}
              onChange={e => setFecha(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Cliente RUC</Form.Label>
            <Form.Control
              type="text"
              value={rucCliente}
              onChange={e => setRucCliente(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Ciudad Entrega</Form.Label>
            <Form.Select
              value={ciudad}
              onChange={e => setCiudad(Number(e.target.value))}
              required
            >
              <option value="">Seleccione</option>
              {ciudades.map(c => (
                <option key={c.codigoCiudad} value={c.codigoCiudad}>
                  {c.nombreCiudad}
                </option>
              ))}
            </Form.Select>
          </Form.Group>

          {detalles.map((d, idx) => (
            <div className="d-flex mb-2" key={idx}>
              <Form.Select
                className="me-2"
                value={d.codigoArticulo}
                onChange={e => handleDetalleChange(idx, 'codigoArticulo', e.target.value)}
                required
              >
                <option value="">Artículo</option>
                {articulos.map(a => (
                  <option key={a.codigoArticulo} value={a.codigoArticulo}>
                    {a.nombreArticulo}
                  </option>
                ))}
              </Form.Select>
              <Form.Control
                className="me-2"
                type="number"
                min="1"
                value={d.cantidad}
                onChange={e => handleDetalleChange(idx, 'cantidad', Number(e.target.value))}
                required
              />
              <Form.Control
                className="me-2"
                type="number"
                readOnly
                value={d.precio}
                placeholder="Precio automático"
              />
              <Button
                variant="danger"
                size="sm"
                onClick={() => setDetalles(detalles.filter((_, i) => i !== idx))}
              >
                Eliminar
              </Button>
            </div>
          ))}

          <Button
            variant="secondary"
            onClick={() => setDetalles([...detalles, { codigoArticulo: '', cantidad: 1, precio: 0 }])}
          >
            Agregar línea
          </Button>

          <div className="mt-3">
            <strong>Total: </strong>${total.toFixed(2)}
          </div>

          {error && <div className="alert alert-danger mt-2">{error}</div>}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onClose}>
            Cancelar
          </Button>
          <Button type="submit" variant="primary">
            Guardar
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
}

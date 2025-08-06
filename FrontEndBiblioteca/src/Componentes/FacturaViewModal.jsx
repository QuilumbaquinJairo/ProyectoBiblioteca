import React, { useEffect, useState } from 'react';
import { Modal, Table } from 'react-bootstrap';
import { authHeaders } from '../utils/auth';

export default function FacturaViewModal({ show, onClose, facturaId }) {
  const [factura, setFactura] = useState(null);

  useEffect(() => {
    if (!facturaId) return;
    fetch(`https://localhost:7172/facturas/${facturaId}`, { headers: authHeaders() })
      .then(r => r.json())
      .then(setFactura);
  }, [facturaId]);

  if (!factura) return null;
  const { fecha, rucCliente, ciudadEntrega, detalles } = factura;

  return (
    <Modal show={show} onHide={onClose} size="lg">
      <Modal.Header closeButton>
        <Modal.Title>Ver Factura {facturaId}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p><strong>Fecha:</strong> {new Date(fecha).toLocaleString()}</p>
        <p><strong>RUC Cliente:</strong> {rucCliente}</p>
        <p><strong>Ciudad de entrega:</strong> {ciudadEntrega}</p>

        <Table bordered>
          <thead><tr><th>Artículo</th><th>Cantidad</th><th>Precio</th><th>Subtotal</th></tr></thead>
          <tbody>
            {detalles.map((d, i) => (
              <tr key={i}>
                <td>{d.codigoArticulo}</td>
                <td>{d.cantidad}</td>
                <td>${d.precio.toFixed(2)}</td>
                <td>${(d.cantidad * d.precio).toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </Table>

      </Modal.Body>
    </Modal>
  );
}

import React, { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import Cookies from 'js-cookie';

export default function ArticuloFormModal({ articulo, onClose, onSuccess }) {
  const isEdit = !!articulo;
  const [nombre, setNombre] = useState('');
  const [saldo, setSaldo] = useState('');
  const [precio, setPrecio] = useState('');

  useEffect(() => {
    if (articulo) {
      setNombre(articulo.nombreArticulo);
      setSaldo(articulo.saldoInventario);
      setPrecio(articulo.precio);
    }
  }, [articulo]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      nombreArticulo: nombre,
      saldoInventario: Number(saldo),
      precio: Number(precio),
    };

    const url = isEdit
      ? `https://localhost:7172/articulos/${articulo.codigoArticulo}`
      : 'https://localhost:7172/articulos';

    const method = isEdit ? 'PUT' : 'POST';

    await fetch(url, {
      method,
      headers: { 
        'Content-Type': 'application/json',
        Authorization: `Bearer ${Cookies.get('auth_token')}`
        
       },
      body: JSON.stringify(payload),
    });

    onSuccess();
    onClose();
  };

  return (
    <Modal show onHide={onClose}>
      <Modal.Header closeButton>
        <Modal.Title>{isEdit ? 'Edit Article' : 'Create Article'}</Modal.Title>
      </Modal.Header>
      <Form onSubmit={handleSubmit}>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Article Name</Form.Label>
            <Form.Control
              type="text"
              value={nombre}
              onChange={(e) => setNombre(e.target.value)}
              required
              placeholder="Enter article name"
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Inventory Balance</Form.Label>
            <Form.Control
              type="number"
              value={saldo}
              onChange={(e) => setSaldo(e.target.value)}
              required
              placeholder="Enter inventory balance"
            />
          </Form.Group>

          <Form.Group>
            <Form.Label>Price</Form.Label>
            <Form.Control
              type="number"
              value={precio}
              onChange={(e) => setPrecio(e.target.value)}
              required
              placeholder="Enter price"
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onClose}>Cancel</Button>
          <Button type="submit" variant="primary">
            {isEdit ? 'Save Changes' : 'Create Article'}
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
}

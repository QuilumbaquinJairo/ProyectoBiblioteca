import React, { useEffect, useRef, useState } from 'react';
import Cookies from 'js-cookie';

export default function ClienteForm({ show, onClose, onSuccess, clienteEdit = null }) {
  const [ruc, setRuc] = useState('');
  const [nombre, setNombre] = useState('');
  const [direccion, setDireccion] = useState('');
  const [error, setError] = useState('');
  const modalRef = useRef(null);
  const modalInstanceRef = useRef(null);

  const isEdit = !!clienteEdit;

  useEffect(() => {
    setRuc(clienteEdit?.ruc || '');
    setNombre(clienteEdit?.nombre || '');
    setDireccion(clienteEdit?.direccion || '');
    setError('');
  }, [clienteEdit]);

  useEffect(() => {
    if (!modalRef.current || !window.bootstrap) return;

    if (!modalInstanceRef.current) {
      modalInstanceRef.current = new window.bootstrap.Modal(modalRef.current, {
        backdrop: 'static',
        keyboard: false
      });

      modalRef.current.addEventListener('hidden.bs.modal', () => {
        onClose();
      });
    }

    show ? modalInstanceRef.current.show() : modalInstanceRef.current.hide();

  }, [show]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const url = isEdit
        ? `https://localhost:7172/clientes/${clienteEdit.ruc}`
        : 'https://localhost:7172/clientes';

      const method = isEdit ? 'PUT' : 'POST';
      const payload = isEdit ? { nombre, direccion } : { ruc, nombre, direccion };

      const res = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${Cookies.get('auth_token')}`,
        },
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error((await res.json()).message || 'Error en la operación');

      onSuccess();
      modalInstanceRef.current.hide(); // Ocultar modal
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="modal fade" ref={modalRef} tabIndex="-1">
      <div className="modal-dialog">
        <div className="modal-content">
          <form onSubmit={handleSubmit}>
            <div className="modal-header">
              <h5 className="modal-title">{isEdit ? 'Editar Cliente' : 'Registrar Cliente'}</h5>
              <button type="button" className="btn-close" onClick={() => modalInstanceRef.current.hide()}></button>
            </div>

            <div className="modal-body">
              {!isEdit && (
                <div className="mb-3">
                  <label className="form-label">RUC</label>
                  <input className="form-control" value={ruc} onChange={(e) => setRuc(e.target.value)} required />
                </div>
              )}
              <div className="mb-3">
                <label className="form-label">Nombre</label>
                <input className="form-control" value={nombre} onChange={(e) => setNombre(e.target.value)} required />
              </div>
              <div className="mb-3">
                <label className="form-label">Dirección</label>
                <input className="form-control" value={direccion} onChange={(e) => setDireccion(e.target.value)} required />
              </div>
              {error && <div className="alert alert-danger">{error}</div>}
            </div>

            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" onClick={() => modalInstanceRef.current.hide()}>
                Cancelar
              </button>
              <button type="submit" className="btn btn-primary">
                {isEdit ? 'Actualizar' : 'Registrar'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

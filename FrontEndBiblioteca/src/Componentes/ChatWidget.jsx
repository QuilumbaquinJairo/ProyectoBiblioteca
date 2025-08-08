import React, { useState, useEffect, useRef } from 'react';
import Cookies from 'js-cookie';

export default function ChatWidget({ onClose }) {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState('');
  const [connected, setConnected] = useState(false);
  const [clientesList, setClientesList] = useState([]); // Para agentes
  const ws = useRef(null);
  const scrollRef = useRef(null);

  const rol = Cookies.get('auth_rol');
  const id = Cookies.get('auth_id');

  // Debug: mostrar valores en consola
  useEffect(() => {
    console.log('=== CHAT WIDGET DEBUG ===');
    console.log('rol:', rol);
    console.log('id:', id);
    console.log('Todas las cookies:', document.cookie);
    console.log('auth_rol cookie:', Cookies.get('auth_rol'));
    console.log('auth_id cookie:', Cookies.get('auth_id'));
    console.log('========================');
  }, []);

  useEffect(() => {
    if (!id || !rol) return;

    const url = `wss://localhost:7172/auth/helpchat/ws?rol=${rol}&id=${id}`;
    console.log('Conectando a:', url);
    
    ws.current = new WebSocket(url);

    ws.current.onopen = () => {
      console.log('WebSocket conectado');
      setConnected(true);
    };

    ws.current.onclose = (event) => {
      console.log('WebSocket cerrado:', event);
      setConnected(false);
    };

    ws.current.onerror = (error) => {
      console.error('WebSocket error:', error);
      setConnected(false);
    };

    ws.current.onmessage = (event) => {
      console.log('Mensaje recibido:', event.data);
      try {
        const { de, mensaje } = JSON.parse(event.data);
        
        // Guardar cliente en lista si somos agente y no existe ya
        if (rol === 'Administrador') {
          setClientesList(prev => {
            if (!prev.includes(de)) {
              return [...prev, de];
            }
            return prev;
          });
        }

        setMessages(prev => [
          ...prev,
          {
            sender: rol === 'Administrador' ? `Empleado ${de}` : 'Agente',
            text: mensaje,
            time: new Date().toLocaleTimeString()
          }
        ]);
      } catch (err) {
        console.error('Error procesando mensaje:', err);
      }
    };

    return () => ws.current?.close();
  }, [id, rol]);

  useEffect(() => {
    scrollRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const sendMessage = (targetClientId = null) => {
    if (!input.trim() || ws.current?.readyState !== WebSocket.OPEN) return;

    const payload = {
      mensaje: input.trim()
    };

    // Si es administrador, usar el cliente seleccionado
    if (rol === 'Administrador' && targetClientId) {
      payload.destinoId = targetClientId;
    }

    console.log('=== FRONTEND SEND DEBUG ===');
    console.log('Input value:', input);
    console.log('Input trimmed:', input.trim());
    console.log('Payload completo:', JSON.stringify(payload));
    console.log('WebSocket readyState:', ws.current?.readyState);
    console.log('=============================');
    
    ws.current.send(JSON.stringify(payload));

    setMessages(prev => [
      ...prev,
      {
        sender: 'Tú',
        text: input.trim(),
        time: new Date().toLocaleTimeString()
      }
    ]);

    setInput('');
  };

  return (
    <div style={{
      position: 'fixed', bottom: '80px', right: '20px',
      width: '400px', height: '600px', backgroundColor: '#fff',
      border: '1px solid #ccc', borderRadius: '8px',
      zIndex: 9999, display: 'flex', flexDirection: 'column'
    }}>
      <div style={{ 
        padding: '12px', 
        background: '#0d6efd', 
        color: '#fff', 
        display: 'flex', 
        justifyContent: 'space-between' 
      }}>
        <strong>{rol === 'Administrador' ? 'Panel Agente' : 'Soporte Online'}</strong>
        <span style={{ cursor: 'pointer' }} onClick={onClose}>✖</span>
      </div>

      {/* Debug info */}
      <div style={{ padding: '8px', backgroundColor: '#f8f9fa', fontSize: '11px', borderBottom: '1px solid #dee2e6' }}>
        <strong>DEBUG:</strong> Rol: {rol} | ID: {id} | Estado: {connected ? 'Conectado' : 'Desconectado'}
      </div>

      {/* Lista de clientes para agente */}
      {rol === 'Administrador' && clientesList.length > 0 && (
        <div style={{ padding: '8px', backgroundColor: '#e9ecef', fontSize: '12px' }}>
          <strong>Clientes activos:</strong> {clientesList.join(', ')}
        </div>
      )}

      <div style={{ flex: 1, overflowY: 'auto', padding: '10px', background: '#f9f9f9' }}>        
        {messages.map((m, idx) => (
          <div key={idx} style={{ 
            marginBottom: '10px',
            padding: '8px',
            backgroundColor: m.sender === 'Tú' ? '#e3f2fd' : '#f5f5f5',
            borderRadius: '6px'
          }}>
            <div><strong>{m.sender}:</strong> {m.text}</div>
            <div style={{ fontSize: '10px', color: '#999', marginTop: '2px' }}>{m.time}</div>
          </div>
        ))}
        <div ref={scrollRef} />
      </div>

      <div style={{ padding: '10px', borderTop: '1px solid #ccc' }}>
        {/* Si es agente y hay clientes, mostrar botones para cada cliente */}
        {rol === 'Administrador' && clientesList.length > 0 && (
          <div style={{ marginBottom: '8px', display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
            {clientesList.map(clienteId => (
              <button 
                key={clienteId}
                className="btn btn-sm btn-outline-secondary"
                onClick={() => sendMessage(clienteId)}
                disabled={!connected || !input.trim()}
              >
                Enviar a {clienteId}
              </button>
            ))}
          </div>
        )}
        
        <div style={{ display: 'flex' }}>
          <input
            type="text"
            className="form-control"
            placeholder="Escribe tu mensaje..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                if (rol === 'Empleado') {
                  sendMessage();
                } else if (clientesList.length > 0) {
                  sendMessage(clientesList[0]); // Enviar al primer cliente por defecto
                }
              }
            }}
          />
          <button 
            className="btn btn-primary ms-2" 
            onClick={() => {
              if (rol === 'Empleado') {
                sendMessage();
              } else if (clientesList.length > 0) {
                sendMessage(clientesList[0]);
              }
            }}
            disabled={!connected || !input.trim()}
          >
            Enviar
          </button>
        </div>
      </div>

      <div style={{ 
        padding: '6px 10px', 
        fontSize: '12px', 
        color: connected ? 'green' : 'red',
        borderTop: '1px solid #eee'
      }}>
        Estado: {connected ? 'Conectado 🟢' : 'Desconectado 🔴'}
      </div>
    </div>
  );
}
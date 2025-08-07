// src/components/HelpButton.jsx
import React, { useState } from 'react';
import ChatWidget from './ChatWidget';

export default function HelpButton() {
  const [visible, setVisible] = useState(false);

  return (
    <>
      {visible && <ChatWidget onClose={() => setVisible(false)} />}
      <button
        onClick={() => setVisible(true)}
        style={{
          position: 'fixed',
          bottom: '20px',
          right: '20px',
          zIndex: 9999,
          borderRadius: '50%',
          backgroundColor: '#0d6efd',
          color: '#fff',
          border: 'none',
          width: '48px',
          height: '48px',
          fontSize: '20px',
          boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
        }}
      >
        ❓
      </button>
    </>
  );
}

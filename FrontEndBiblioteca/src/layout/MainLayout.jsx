// src/layout/MainLayout.jsx
import React from 'react';
import Navbar from '../Componentes/Navbar';
import { Outlet } from 'react-router-dom';

export default function MainLayout() {
  return (
    <div>
      <Navbar />
      <main className="p-6">
        <Outlet /> {/* Aquí se renderizan las rutas hijas */}
      </main>
    </div>
  );
}

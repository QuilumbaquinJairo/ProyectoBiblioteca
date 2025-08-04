// src/components/Navbar.jsx
import React from 'react';
import { Link } from 'react-router-dom';

export default function Navbar() {
  return (
    <nav className="bg-white shadow px-6 py-4 flex justify-between items-center">
      <div className="font-bold text-xl text-gray-800">BookWise</div>
      <div className="space-x-6">
        <Link to="/clientes" className="text-gray-700 hover:text-blue-600">Clientes</Link>
        <button className="text-gray-700 hover:text-blue-600">Cerrar sesión</button>
      </div>
    </nav>
  );
}

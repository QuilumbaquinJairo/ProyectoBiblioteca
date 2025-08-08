// src/components/Navbar.jsx
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';

export default function Navbar() {
  const navigate = useNavigate();
  const [isCollapsed, setIsCollapsed] = useState(true);

  const handleLogout = () => {
    Cookies.remove('auth_token');
    navigate('/login');
  };

  const toggleNavbar = () => {
    setIsCollapsed(!isCollapsed);
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light shadow-sm">
      <div className="container-fluid">
        {/* Brand */}
        <Link className="navbar-brand fw-bold text-primary" to="/clientes">
          BookWise
        </Link>

        {/* Botón hamburguesa - solo visible en móviles */}
        <button
          className="navbar-toggler d-lg-none"
          type="button"
          onClick={toggleNavbar}
          aria-controls="navbarNav"
          aria-expanded={!isCollapsed}
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        {/* Menú - visible en escritorio, colapsable en móviles */}
        <div 
          className={`navbar-collapse d-lg-flex ${isCollapsed ? 'd-none d-lg-flex' : 'd-flex'}`} 
          id="navbarNav"
        >
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <Link to="/clientes" className="nav-link fw-semibold text-dark">
                Clientes
              </Link>
            </li>
            <li className="nav-item">
              <Link to="/articulos" className="nav-link fw-semibold text-dark">
                Artículos
              </Link>
            </li>
            <li className="nav-item">
              <Link to="/facturas" className="nav-link fw-semibold text-dark">
                Facturas
              </Link>
            </li>
            <li className="nav-item">
              <Link to="/ciudades" className="nav-link fw-semibold text-dark">
                Ciudades
              </Link>
            </li>
            <li className="nav-item">
              <Link to="/reportes" className="nav-link fw-semibold text-dark">
                Reportes
              </Link>
            </li>
          </ul>

          <button 
            onClick={handleLogout} 
            className="btn btn-outline-danger btn-sm"
          >
            Cerrar sesión
          </button>
        </div>
      </div>
    </nav>
  );
}
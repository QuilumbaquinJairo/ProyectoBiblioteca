// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './Login/Login';
import Register from './Login/Register';
import ClienteView from './pages/ClienteView'; // <-- NUEVA VISTA
import RequireAuth from './auth/RequireAuth'; // <-- Middleware auth

export default function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* RUTA PROTEGIDA */}
        <Route
          path="/clientes"
          element={
            <RequireAuth>
              <ClienteView />
            </RequireAuth>
          }
        />
      </Routes>
    </Router>
  );
}


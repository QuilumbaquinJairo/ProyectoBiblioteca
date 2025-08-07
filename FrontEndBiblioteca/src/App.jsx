// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './layout/MainLayout';
import ClienteView from './pages/ClienteView';
import ArticulosView from './pages/ArticulosView';
import Login from './Login/Login';
import Register from './Login/Register';
import RequireAuth from './auth/RequireAuth'; // token check
import FacturasView from './pages/FacturasView';
import CiudadView from './pages/CiudadView';
import HelpButton from './Componentes/HelpButton';

export default function App() {
  return (
    <>  
    <Router>
      <Routes>
        <Route path="/" element={<Navigate to="/clientes" />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        <Route
          path="/"
          element={
            <RequireAuth>
              <MainLayout />
            </RequireAuth>
          }
        >
          <Route path="clientes" element={<ClienteView />} />
          <Route path="articulos" element={<ArticulosView />} />
          <Route path="facturas" element={<FacturasView />} />
          <Route path="ciudades" element={<CiudadView />} />
        </Route>
      </Routes>
    </Router>
    <HelpButton />
    </>
  );
}

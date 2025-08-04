// src/auth/RequireAuth.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import Cookies from 'js-cookie';

export default function RequireAuth({ children }) {
  const token = Cookies.get('auth_token');

  if (!token) {
    return <Navigate to="/login" />;
  }

  return children;
}

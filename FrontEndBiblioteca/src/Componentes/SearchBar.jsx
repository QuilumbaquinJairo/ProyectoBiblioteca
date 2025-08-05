// src/components/SearchBar.jsx
import React from 'react';

export default function SearchBar({ onSearch }) {
  return (
    <input
      type="text"
      placeholder="Buscar por RUC o nombre"
      onChange={(e) => onSearch(e.target.value)}
      className="form-control w-100 w-md-50"
    />
  );
}

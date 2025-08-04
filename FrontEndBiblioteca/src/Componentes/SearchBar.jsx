// src/components/SearchBar.jsx
import React from 'react';

export default function SearchBar({ onSearch }) {
  return (
    <input
      type="text"
      placeholder="Buscar por RUC o nombre"
      onChange={(e) => onSearch(e.target.value)}
      className="w-full max-w-md px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-400"
    />
  );
}

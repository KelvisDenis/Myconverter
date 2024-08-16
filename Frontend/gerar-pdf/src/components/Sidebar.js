
// src/components/Sidebar.js
import React, { useState } from 'react';
import '../css/componentscss/Sidebar.css';
import { Link } from 'react-router-dom';

export default function Sidebar(){
  const [isOpen, setIsOpen] = useState(true);

  const toggleSidebar = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div className={`sidebar ${isOpen ? 'open' : 'closed'}`}>
        
      <button className="toggle-btn" onClick={toggleSidebar}>
        {isOpen ? '=' : '='}
      </button>
      <ul className="sidebar-list">
        <li className="sidebar-item">
          <Link to="/">Home</Link>
        </li>
        <li className="sidebar-item">
          <Link to="/covertpdfToCsv">PDF - CSV</Link>
        </li>
        <li className="sidebar-item">
          <Link to="/convertToDocs">PDF - DOCs</Link>
        </li>
        <li className="sidebar-item">
          <Link to="/juntarPDF">Juntar PDF</Link>
        </li>
      </ul>
    </div>
  );
};


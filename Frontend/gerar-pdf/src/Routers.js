import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import ConvertToCsv from './pages/ConvToCsv.js';
import ConvertToDocs from './pages/ConvToDocs.js';
import JuntarPdf from './pages/JuntarPdf.js';
import Sidebar from './components/Sidebar.js';
import Home from './pages/Home.js';

export default function Routers(){
    return(
        <Router>
            <Routes>
                <Route path='/' element={<Home/>}/>
                <Route path='/covertpdfToCsv' element={<ConvertToCsv/>}/>
                <Route path='/convertToDocs' element={<ConvertToDocs/>}/>
                <Route path='/juntarPDF' element={<JuntarPdf/>}/>
            </Routes>
            <Sidebar/>
        </Router>
    )
}
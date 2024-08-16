import React from "react"
import '../css/pagescss/Home.css'
import { Link } from "react-router-dom"

export default function Home(){
   
    return(
        <div className="home-container">
      <div className="card">
        <h2>Converter para CSV</h2>
        <Link to={'/covertpdfToCsv'}>
            <p>Conteúdo do Card 1</p>
        </Link>
      </div>
      <div className="card">
        <h2>Converter para DOCS</h2>
        <Link to={'/convertToDocs'}>
            <p>Conteúdo do Card 2</p>
        </Link>
      </div>
      <div className="card">
        <h2>Juntar PDF</h2>
        <Link to={'/juntarPDF'}>
            <p>Conteúdo do Card 3</p>
        </Link>
      </div>
    </div>
    )
}
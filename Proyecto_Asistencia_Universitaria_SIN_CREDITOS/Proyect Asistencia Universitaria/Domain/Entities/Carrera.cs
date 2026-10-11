using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Carrera
    {
        public int IdCarrera { get; set; }
        public string? NombreCarrera { get; set; }

       
        public int IdFacultad { get; set; }
        public string? NombreFacultad { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.DTos
{
    public class RegistroAsistenciaDto
    {
        public int IdSesion { get; set; }
        public int IdEstudiante { get; set; }
        public string? Carnet { get; set; }
        public string? NombreCompleto { get; set; }
        public string? EstadoAsistencia { get; set; } 
    }
}

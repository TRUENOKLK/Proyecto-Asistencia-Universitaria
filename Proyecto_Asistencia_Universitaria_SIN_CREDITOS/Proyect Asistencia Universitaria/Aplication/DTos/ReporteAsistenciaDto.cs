using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.DTos
{
    public class ReporteAsistenciaDto
    {
        public DateTime FechaSesion { get; set; }
        public string? Tema { get; set; }
        public string? Carnet { get; set; }
        public string? Estudiante { get; set; }
        public string? EstadoAsistencia { get; set; }
    }
}

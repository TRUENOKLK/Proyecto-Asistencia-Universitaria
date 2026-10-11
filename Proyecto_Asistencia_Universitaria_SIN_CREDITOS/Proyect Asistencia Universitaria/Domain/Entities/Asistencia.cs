using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Asistencia
    {
        public int IdAsistencia { get; set; }
        public int IdSesion { get; set; }
        public int IdEstudiante { get; set; }
        public string? EstadoAsistencia { get; set; } 
        public DateTime FechaRegistro { get; set; }

        
        public DateTime FechaSesion { get; set; }
        public string? TemaSesion { get; set; }
        public string? Carnet { get; set; }
        public string? Estudiante { get; set; }
    }
}

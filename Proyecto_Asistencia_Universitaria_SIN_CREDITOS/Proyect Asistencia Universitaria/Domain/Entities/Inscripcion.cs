using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Inscripcion
    {
        public int IdInscripcion { get; set; }
        public int IdEstudiante { get; set; }
        public int IdGrupo { get; set; }
        public DateTime FechaInscripcion { get; set; }

        
        public string? CarnetEstudiante { get; set; }
        public string? NombreCompletoEstudiante { get; set; }
    }
}

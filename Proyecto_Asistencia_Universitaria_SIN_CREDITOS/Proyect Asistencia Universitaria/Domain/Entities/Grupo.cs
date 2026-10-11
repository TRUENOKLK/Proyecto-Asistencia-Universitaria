using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Grupo
    {
        public int IdGrupo { get; set; }
        public string? NombreGrupo { get; set; }
        public int IdAsignatura { get; set; }
        public int IdProfesor { get; set; }
        public int IdPeriodo { get; set; }

        
        public string? NombreAsignatura { get; set; }
        public string? Docente { get; set; }
        public string? NombrePeriodo { get; set; }

        public string? DiaSemana { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
    }
}

using System;

namespace Aplication.DTos
{
    public class MiAsistenciaDto
    {
        public DateTime FechaSesion { get; set; }
        public string? Tema { get; set; }
        public string? NombreAsignatura { get; set; }
        public string? NombreGrupo { get; set; }
        public string? EstadoAsistencia { get; set; }
    }
}

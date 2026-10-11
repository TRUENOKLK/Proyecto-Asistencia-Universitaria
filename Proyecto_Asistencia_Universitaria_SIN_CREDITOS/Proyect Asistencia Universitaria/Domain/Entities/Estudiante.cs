using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Estudiante
    {
        public int IdEstudiante { get; set; }
        public string? Carnet { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public int IdCarrera { get; set; }
        public int IdUsuario { get; set; }

        
        public string? NombreCarrera { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; }

        
        public string? PasswordHash { get; set; }
    }
}
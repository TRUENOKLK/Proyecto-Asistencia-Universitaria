using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Profesor
    {
        public int IdProfesor { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }

        
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool Activo { get; set; }
    }
}
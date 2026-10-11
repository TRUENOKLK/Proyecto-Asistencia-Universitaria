using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int IdRol { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

       
        public string? NombreRol { get; set; }
    }
}

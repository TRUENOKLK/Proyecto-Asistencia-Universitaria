using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.DTos
{
    public class UsuarioSesionDto
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? NombreRol { get; set; }
        public bool Activo { get; set; }
    }
}
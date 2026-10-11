using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Sesion
    {
        public int IdSesion { get; set; }
        public int IdGrupo { get; set; }
        public DateTime FechaSesion { get; set; }
        public string? Tema { get; set; }
    }
}

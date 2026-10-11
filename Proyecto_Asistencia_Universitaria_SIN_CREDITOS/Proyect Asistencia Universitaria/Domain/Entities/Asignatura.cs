using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Asignatura
    {
        public int IdAsignatura { get; set; }
        public string? Codigo { get; set; }
        public string? NombreAsignatura { get; set; }
        public int Creditos { get; set; }
        public int IdCarrera { get; set; }
        public string? NombreCarrera { get; set; }
    }
}

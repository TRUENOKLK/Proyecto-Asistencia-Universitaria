using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Periodo
    {
        public int IdPeriodo { get; set; }
        public string? NombrePeriodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplication.DTos
{
    public class PeriodoDto
    {
        public int IdPeriodo { get; set; }

        [Required(ErrorMessage = "El nombre del periodo es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre del Periodo (Ej. 2026-I)")]
        public string? NombrePeriodo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
    }
}

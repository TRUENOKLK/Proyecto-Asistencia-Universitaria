using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplication.DTos
{
    public class AsignaturaDto
    {
        public int IdAsignatura { get; set; }

        [Required(ErrorMessage = "El código es obligatorio papoi")]
        [StringLength(20)]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "El nombre de la asignatura es obligatorio")]
        [StringLength(100)]
        public string? NombreAsignatura { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una carrera")]
        [Display(Name = "Carrera")]
        public int IdCarrera { get; set; }

        public string? NombreCarrera { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplication.DTos
{
    public class CarreraDto
    {
        public int IdCarrera { get; set; }

        [Required(ErrorMessage = "El nombre de la carrera es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre de la Carrera")]
        public string? NombreCarrera { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Facultad")]
        [Display(Name = "Facultad")]
        public int IdFacultad { get; set; }

        public string? NombreFacultad { get; set; } 
    }
}
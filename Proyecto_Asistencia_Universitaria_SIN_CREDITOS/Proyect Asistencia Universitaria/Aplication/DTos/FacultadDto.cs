using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplication.DTos
{
    public class FacultadDto
    {
        public int IdFacultad { get; set; }

        [Required(ErrorMessage = "El nombre de la facultad es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre de la Facultad")]
        public string? NombreFacultad { get; set; }
    }
}
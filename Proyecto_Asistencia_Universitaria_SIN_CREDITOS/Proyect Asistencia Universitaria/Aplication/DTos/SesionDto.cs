using System;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Aplication.Validation;

namespace Aplication.DTos
{
    public class SesionDto
    {
        public int IdSesion { get; set; }
        public int IdGrupo { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [FechaNoAnteriorAHoy]
        public DateTime FechaSesion { get; set; }

        [Required(ErrorMessage = "El tema es obligatorio")]
        public string? Tema { get; set; }
    }
}

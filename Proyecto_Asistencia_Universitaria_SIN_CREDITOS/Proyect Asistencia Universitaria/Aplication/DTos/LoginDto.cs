using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplication.DTos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo institucional es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo con formato válido")]
        [StringLength(150, ErrorMessage = "El correo es demasiado largo")]
        [Display(Name = "Correo institucional")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }
    }
}

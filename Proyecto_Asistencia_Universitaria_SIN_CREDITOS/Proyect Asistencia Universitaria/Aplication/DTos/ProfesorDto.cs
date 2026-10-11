using System.ComponentModel.DataAnnotations;
using Aplication.Validation;

namespace Aplication.DTos
{
    public class ProfesorDto
    {
        public int IdProfesor { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100)]
        [Display(Name = "Nombres")]
        public string? Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string? Apellidos { get; set; }

        [Phone(ErrorMessage = "Ingresa un teléfono válido")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(150)]
        [Display(Name = "Correo institucional")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña temporal es obligatoria")]
        [PasswordPolicy]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña temporal")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Debes confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare(nameof(PasswordHash), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string? ConfirmarPassword { get; set; }

        public bool Activo { get; set; }
    }
}
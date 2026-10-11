using System.ComponentModel.DataAnnotations;
using Aplication.Validation;

namespace Aplication.DTos
{
    public class EstudianteDto
    {
        public int IdEstudiante { get; set; }

        [Required(ErrorMessage = "El carnet es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Carnet Universitario")]
        public string? Carnet { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100)]
        [Display(Name = "Nombres")]
        public string? Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string? Apellidos { get; set; }

        [Required(ErrorMessage = "La carrera es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una carrera válida")]
        [Display(Name = "Carrera")]
        public int IdCarrera { get; set; }

        
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

       
        public string? NombreCarrera { get; set; }
        public bool Activo { get; set; }
    }
}
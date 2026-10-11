using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplication.DTos
{
    public class GrupoDto : IValidatableObject
    {
        public int IdGrupo { get; set; }

        [Required(ErrorMessage = "El nombre del grupo es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Nombre del Grupo")]
        public string? NombreGrupo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una asignatura")]
        [Display(Name = "Asignatura")]
        public int IdAsignatura { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un profesor")]
        [Display(Name = "Profesor")]
        public int IdProfesor { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un periodo")]
        [Display(Name = "Periodo")]
        public int IdPeriodo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el día de la semana")]
        [Display(Name = "Día de la Semana")]
        public string? DiaSemana { get; set; }

        [Required(ErrorMessage = "Debe indicar la hora de inicio")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora de Inicio")]
        public TimeSpan? HoraInicio { get; set; }

        [Required(ErrorMessage = "Debe indicar la hora de fin")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora de Fin")]
        public TimeSpan? HoraFin { get; set; }

        public string? NombreAsignatura { get; set; }
        public string? Docente { get; set; }
        public string? NombrePeriodo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HoraInicio.HasValue && HoraFin.HasValue && HoraFin.Value <= HoraInicio.Value)
            {
                yield return new ValidationResult(
                    "La hora de fin debe ser posterior a la hora de inicio.",
                    new[] { nameof(HoraFin) });
            }
        }
    }
}

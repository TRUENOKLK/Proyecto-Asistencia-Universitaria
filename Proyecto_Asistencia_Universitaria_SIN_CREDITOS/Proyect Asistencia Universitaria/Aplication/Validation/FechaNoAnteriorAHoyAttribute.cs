using System.ComponentModel.DataAnnotations;

namespace Aplication.Validation
{
    public class FechaNoAnteriorAHoyAttribute : ValidationAttribute
    {
        public FechaNoAnteriorAHoyAttribute()
        {
            ErrorMessage = "La fecha no puede ser anterior al día de hoy.";
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateTime fecha) return false;
            return fecha.Date >= DateTime.Today;
        }
    }
}

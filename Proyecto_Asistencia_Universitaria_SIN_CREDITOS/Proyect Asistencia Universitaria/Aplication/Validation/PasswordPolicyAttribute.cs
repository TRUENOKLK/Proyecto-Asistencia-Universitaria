using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Aplication.Validation
{

    public class PasswordPolicyAttribute : ValidationAttribute
    {
        private static readonly Regex Mayuscula = new(@"[A-ZÁÉÍÓÚÑ]", RegexOptions.Compiled);
        private static readonly Regex Minuscula = new(@"[a-záéíóúñ]", RegexOptions.Compiled);
        private static readonly Regex Numero = new(@"[0-9]", RegexOptions.Compiled);
        private static readonly Regex Especial = new(@"[^A-Za-z0-9]", RegexOptions.Compiled);

        public const int LongitudMinima = 8;

        public PasswordPolicyAttribute()
        {
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres, e incluir una mayúscula, " +
                           "una minúscula, un número y un carácter especial (ej: #, @, %, *).";
        }

        public override bool IsValid(object? value)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password)) return false;

            return password.Length >= LongitudMinima
                && Mayuscula.IsMatch(password)
                && Minuscula.IsMatch(password)
                && Numero.IsMatch(password)
                && Especial.IsMatch(password);
        }
    }
}

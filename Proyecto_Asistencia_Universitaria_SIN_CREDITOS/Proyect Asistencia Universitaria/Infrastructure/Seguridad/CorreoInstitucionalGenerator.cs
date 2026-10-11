using Aplication.Interface;
using System;
using System.Globalization;
using System.Text;

namespace Infrastructure.Seguridad
{
 
    public class CorreoInstitucionalGenerator : ICorreoInstitucionalGenerator
    {
        public const string DominioEstudiante = "est.uni.edu.ni";
        public const string DominioProfesor = "uni.edu.ni";
        public const string DominioAdministrador = "admin.uni.edu.ni";

        public string ObtenerDominio(string rol) => rol?.Trim().ToLowerInvariant() switch
        {
            "estudiante" => DominioEstudiante,
            "profesor" => DominioProfesor,
            "administrador" => DominioAdministrador,
            _ => DominioProfesor
        };

        public string Generar(string nombres, string apellidos, string rol, int intento = 1)
        {
            var primerNombre = NormalizarParte(PrimeraPalabra(nombres));
            var primerApellido = NormalizarParte(PrimeraPalabra(apellidos));

            if (string.IsNullOrWhiteSpace(primerNombre)) primerNombre = "usuario";
            if (string.IsNullOrWhiteSpace(primerApellido)) primerApellido = "uni";

            var local = $"{primerNombre}.{primerApellido}";
            if (intento > 1) local += intento.ToString();

            return $"{local}@{ObtenerDominio(rol)}";
        }

        private static string PrimeraPalabra(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            var partes = texto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length > 0 ? partes[0] : string.Empty;
        }

        
        private static string NormalizarParte(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalizado)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria == UnicodeCategory.NonSpacingMark) continue; 

                if (char.IsLetterOrDigit(c))
                    builder.Append(char.ToLowerInvariant(c));
            }

            return builder.ToString();
        }
    }
}

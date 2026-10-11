namespace Aplication.Interface
{
    public interface ICorreoInstitucionalGenerator
    {
        /// <summary>
        /// Genera el correo institucional. Cuando <paramref name="intento"/> es
        /// mayor que 1, agrega un sufijo numérico para desambiguar homónimos.
        /// </summary>
        string Generar(string nombres, string apellidos, string rol, int intento = 1);
    }
}

using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplication.Interface
{
    
    public static class AccionAuditoria
    {
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string Insertar = "INSERTAR";
        public const string Editar = "EDITAR";
        public const string Activar = "ACTIVAR";
        public const string Desactivar = "DESACTIVAR";
        public const string Eliminar = "ELIMINAR";
    }

    public interface IAuditoriaRepository
    {
        Task RegistrarAsync(string email, string accion, string modulo, string? procedimiento, string? detalle);
        Task<IEnumerable<RegistroAuditoria>> ListarAsync(string? accion);
        Task<int> ObtenerContadorSesionesAsync();
    }
}

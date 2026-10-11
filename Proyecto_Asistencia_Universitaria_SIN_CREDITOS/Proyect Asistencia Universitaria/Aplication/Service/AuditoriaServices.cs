using Aplication.DTos;
using Aplication.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplication.Service
{
    public class AuditoriaServices
    {
        private readonly IAuditoriaRepository _repository;
        public AuditoriaServices(IAuditoriaRepository repository) => _repository = repository;

        
        public Task Registrar(string? email, string accion, string modulo, string? procedimiento = null, string? detalle = null) =>
            _repository.RegistrarAsync(email ?? "desconocido", accion, modulo, procedimiento, detalle);

        public async Task<IEnumerable<AuditoriaDto>> Listar(string? accion)
        {
            var lista = await _repository.ListarAsync(string.IsNullOrWhiteSpace(accion) ? null : accion);
            return lista.Select(r => new AuditoriaDto
            {
                IdAuditoria = r.IdAuditoria,
                Email = r.Email,
                Accion = r.Accion,
                Modulo = r.Modulo,
                Procedimiento = r.Procedimiento,
                Detalle = r.Detalle,
                FechaHora = r.FechaHora
            });
        }

        public Task<int> ObtenerContadorSesiones() => _repository.ObtenerContadorSesionesAsync();
    }
}

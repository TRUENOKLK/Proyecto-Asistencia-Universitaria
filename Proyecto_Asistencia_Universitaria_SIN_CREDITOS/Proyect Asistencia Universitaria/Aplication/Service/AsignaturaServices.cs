using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Service
{
    public class AsignaturaServices
    {
        private readonly IAsignaturaRepository _repository;
        public AsignaturaServices(IAsignaturaRepository repository) => _repository = repository;

        // La BD todavía requiere un valor NOT NULL en Asignaturas.Creditos,
        // pero ya no se le pide al usuario en el formulario, así que se
        // guarda con un valor fijo interno para no romper el INSERT/UPDATE.
        private const int CreditosPorDefecto = 1;

        public async Task<IEnumerable<AsignaturaDto>> ListarAsignaturas()
        {
            var list = await _repository.ListarAsignaturasAsync();
            return list.Select(a => new AsignaturaDto
            {
                IdAsignatura = a.IdAsignatura,
                Codigo = a.Codigo,
                NombreAsignatura = a.NombreAsignatura,
                IdCarrera = a.IdCarrera,
                NombreCarrera = a.NombreCarrera
            });
        }

        public async Task NuevaAsignatura(AsignaturaDto dto) =>
            await _repository.InsertarAsignaturaAsync(new Asignatura { Codigo = dto.Codigo, NombreAsignatura = dto.NombreAsignatura, Creditos = CreditosPorDefecto, IdCarrera = dto.IdCarrera });

        public async Task<AsignaturaDto?> ObtenerPorId(int idAsignatura)
        {
            var lista = await ListarAsignaturas();
            return lista.FirstOrDefault(x => x.IdAsignatura == idAsignatura);
        }

        public async Task EditarAsignatura(AsignaturaDto dto) =>
            await _repository.EditarAsignaturaAsync(new Asignatura { IdAsignatura = dto.IdAsignatura, Codigo = dto.Codigo, NombreAsignatura = dto.NombreAsignatura, Creditos = CreditosPorDefecto, IdCarrera = dto.IdCarrera });

        public async Task EliminarAsignatura(int idAsignatura) => await _repository.EliminarAsignaturaAsync(idAsignatura);
    }
}

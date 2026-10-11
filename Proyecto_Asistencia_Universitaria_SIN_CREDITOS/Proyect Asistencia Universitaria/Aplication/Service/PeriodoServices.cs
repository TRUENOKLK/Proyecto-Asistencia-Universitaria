using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Service
{
    public class PeriodoServices
    {
        private readonly IPeriodoRepository _repository;
        public PeriodoServices(IPeriodoRepository repository) => _repository = repository;

        public async Task<IEnumerable<PeriodoDto>> ListarPeriodos()
        {
            var list = await _repository.ListarPeriodosAsync();
            return list.Select(p => new PeriodoDto
            {
                IdPeriodo = p.IdPeriodo,
                NombrePeriodo = p.NombrePeriodo,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin
            });
        }

        public async Task NuevoPeriodo(PeriodoDto dto) =>
            await _repository.InsertarPeriodoAsync(new Periodo { NombrePeriodo = dto.NombrePeriodo, FechaInicio = dto.FechaInicio, FechaFin = dto.FechaFin });

        public async Task<PeriodoDto?> ObtenerPorId(int idPeriodo)
        {
            var lista = await _repository.ListarPeriodosAsync();
            var p = lista.FirstOrDefault(x => x.IdPeriodo == idPeriodo);
            return p == null ? null : new PeriodoDto { IdPeriodo = p.IdPeriodo, NombrePeriodo = p.NombrePeriodo, FechaInicio = p.FechaInicio, FechaFin = p.FechaFin };
        }

        public async Task EditarPeriodo(PeriodoDto dto) =>
            await _repository.EditarPeriodoAsync(new Periodo { IdPeriodo = dto.IdPeriodo, NombrePeriodo = dto.NombrePeriodo, FechaInicio = dto.FechaInicio, FechaFin = dto.FechaFin });

        public async Task EliminarPeriodo(int idPeriodo) => await _repository.EliminarPeriodoAsync(idPeriodo);
    }
}


using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Service
{
    public class FacultadServices
    {
        private readonly IFacultadRepository _repository;

        public FacultadServices(IFacultadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FacultadDto>> ListarFacultades()
        {
            var listar = await _repository.ListarFacultadesAsync();
            return listar.Select(f => new FacultadDto
            {
                IdFacultad = f.IdFacultad,
                NombreFacultad = f.NombreFacultad
            });
        }

        public async Task NuevaFacultad(FacultadDto dto)
        {
            var oFacultad = new Facultad
            {
                NombreFacultad = dto.NombreFacultad
            };
            await _repository.InsertarFacultadAsync(oFacultad);
        }

        public async Task<FacultadDto?> ObtenerPorId(int idFacultad)
        {
            var lista = await _repository.ListarFacultadesAsync();
            var f = lista.FirstOrDefault(x => x.IdFacultad == idFacultad);
            return f == null ? null : new FacultadDto { IdFacultad = f.IdFacultad, NombreFacultad = f.NombreFacultad };
        }

        public async Task EditarFacultad(FacultadDto dto) =>
            await _repository.EditarFacultadAsync(new Facultad { IdFacultad = dto.IdFacultad, NombreFacultad = dto.NombreFacultad });

        public async Task EliminarFacultad(int idFacultad) =>
            await _repository.EliminarFacultadAsync(idFacultad);
    }
}
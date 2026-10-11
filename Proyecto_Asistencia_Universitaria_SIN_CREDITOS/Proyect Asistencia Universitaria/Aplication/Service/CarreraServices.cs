using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Service
{
    public class CarreraServices
    {
        private readonly ICarreraRepository _repository;

        public CarreraServices(ICarreraRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CarreraDto>> ListarCarreras()
        {
            var listar = await _repository.ListarCarrerasAsync();

            return listar.Select(c => new CarreraDto
            {
                IdCarrera = c.IdCarrera,
                NombreCarrera = c.NombreCarrera,
                IdFacultad = c.IdFacultad,
                NombreFacultad = c.NombreFacultad
            });
        }

        public async Task NuevaCarrera(CarreraDto dto)
        {
            var oCarrera = new Carrera
            {
                NombreCarrera = dto.NombreCarrera,
                IdFacultad = dto.IdFacultad
            };

            await _repository.InsertarCarreraAsync(oCarrera);
        }

        public async Task<CarreraDto?> ObtenerPorId(int idCarrera)
        {
            var lista = await _repository.ListarCarrerasAsync();
            var c = lista.FirstOrDefault(x => x.IdCarrera == idCarrera);
            return c == null ? null : new CarreraDto { IdCarrera = c.IdCarrera, NombreCarrera = c.NombreCarrera, IdFacultad = c.IdFacultad, NombreFacultad = c.NombreFacultad };
        }

        public async Task EditarCarrera(CarreraDto dto) =>
            await _repository.EditarCarreraAsync(new Carrera { IdCarrera = dto.IdCarrera, NombreCarrera = dto.NombreCarrera, IdFacultad = dto.IdFacultad });

        public async Task EliminarCarrera(int idCarrera) => await _repository.EliminarCarreraAsync(idCarrera);
    }
}
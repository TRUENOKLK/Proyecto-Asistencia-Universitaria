using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface ICarreraRepository
    {
        Task<IEnumerable<Carrera>> ListarCarrerasAsync();
        Task InsertarCarreraAsync(Carrera oCarrera);
        Task EditarCarreraAsync(Carrera oCarrera);
        Task EliminarCarreraAsync(int idCarrera);
    }
}
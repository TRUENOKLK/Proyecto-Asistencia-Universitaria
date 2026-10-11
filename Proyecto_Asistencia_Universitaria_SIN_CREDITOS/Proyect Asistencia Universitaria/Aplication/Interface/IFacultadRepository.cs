using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface IFacultadRepository
    {
        Task<IEnumerable<Facultad>> ListarFacultadesAsync();
        Task InsertarFacultadAsync(Facultad oFacultad);
        Task EditarFacultadAsync(Facultad oFacultad);
        Task EliminarFacultadAsync(int idFacultad);
    }
}
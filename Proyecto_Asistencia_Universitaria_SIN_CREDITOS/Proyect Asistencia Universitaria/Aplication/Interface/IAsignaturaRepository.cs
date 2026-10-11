using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface IAsignaturaRepository
    {
        Task<IEnumerable<Asignatura>> ListarAsignaturasAsync();
        Task InsertarAsignaturaAsync(Asignatura oAsignatura);
        Task EditarAsignaturaAsync(Asignatura oAsignatura);
        Task EliminarAsignaturaAsync(int idAsignatura);
    }
}

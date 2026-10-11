using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface IProfesorRepository
    {
        Task<IEnumerable<Profesor>> ListarProfesoresAsync();
        Task<Profesor?> ObtenerPorIdAsync(int idProfesor);
        Task InsertarProfesorAsync(Profesor oProfesor);
        Task EditarProfesorAsync(Profesor oProfesor);
        Task CambiarEstadoAsync(int idProfesor, bool activo);
    }
}
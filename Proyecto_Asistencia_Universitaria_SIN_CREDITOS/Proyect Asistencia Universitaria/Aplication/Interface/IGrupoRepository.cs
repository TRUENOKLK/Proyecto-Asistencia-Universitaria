using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface IGrupoRepository
    {
        Task<IEnumerable<Grupo>> ListarTodosLosGruposAsync();
        Task<IEnumerable<Grupo>> ListarGruposPorProfesorAsync(int idProfesor);
        Task<Grupo?> ObtenerPorIdAsync(int idGrupo);
        Task InsertarGrupoAsync(Grupo oGrupo);
        Task EditarGrupoAsync(Grupo oGrupo);
        Task EliminarGrupoAsync(int idGrupo);

        Task<IEnumerable<Estudiante>> ListarEstudiantesInscritosAsync(int idGrupo);
        Task<IEnumerable<Estudiante>> ListarEstudiantesDisponiblesAsync(int idGrupo);
        Task InscribirEstudianteAsync(int idGrupo, int idEstudiante);
        Task DesinscribirEstudianteAsync(int idGrupo, int idEstudiante);
    }
}

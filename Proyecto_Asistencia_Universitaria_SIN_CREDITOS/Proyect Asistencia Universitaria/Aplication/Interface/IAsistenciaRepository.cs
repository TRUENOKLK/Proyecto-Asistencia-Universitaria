using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface IAsistenciaRepository
    {
        Task<IEnumerable<Estudiante>> ListarEstudiantesPorGrupoAsync(int idGrupo);
        Task RegistrarAsistenciaAsync(Asistencia asistencia);
        Task<IEnumerable<Asistencia>> ReporteAsistenciaAsync(int idGrupo);
    }
}

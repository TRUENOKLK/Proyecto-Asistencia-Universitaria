using System;
using System.Collections.Generic;
using System.Text;
using Aplication.DTos;
using Domain.Entities;

namespace Aplication.Interface
{
    public interface IEstudianteRepository
    {
        Task<IEnumerable<Estudiante>> ListarEstudiantesAsync();
        Task<Estudiante?> ObtenerPorIdAsync(int idEstudiante);
        Task InsertarEstudianteAsync(Estudiante oEstudiante);
        Task EditarEstudianteAsync(Estudiante oEstudiante);
        Task CambiarEstadoAsync(int idEstudiante, bool activo);
        Task InsertarMasivoAsync(IEnumerable<Estudiante> listaEstudiantes);
        Task<IEnumerable<MiAsistenciaDto>> ObtenerAsistenciaPorEmailAsync(string email);
    }
}
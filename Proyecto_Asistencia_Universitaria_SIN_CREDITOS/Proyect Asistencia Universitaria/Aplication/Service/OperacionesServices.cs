using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Service
{
    public class OperacionesServices
    {
        private readonly ISesionRepository _sesionRepo;
        private readonly IAsistenciaRepository _asistenciaRepo;

        public OperacionesServices(ISesionRepository sesionRepo, IAsistenciaRepository asistenciaRepo)
        {
            _sesionRepo = sesionRepo;
            _asistenciaRepo = asistenciaRepo;
        }

        public async Task CrearSesion(SesionDto dto) =>
            await _sesionRepo.CrearSesionAsync(new Sesion { IdGrupo = dto.IdGrupo, FechaSesion = dto.FechaSesion, Tema = dto.Tema });

        public async Task<IEnumerable<SesionDto>> ListarSesiones(int idGrupo)
        {
            var list = await _sesionRepo.ListarSesionesPorGrupoAsync(idGrupo);
            return list.Select(s => new SesionDto { IdSesion = s.IdSesion, FechaSesion = s.FechaSesion, Tema = s.Tema });
        }

        public async Task<IEnumerable<RegistroAsistenciaDto>> ObtenerEstudiantesParaAsistencia(int idGrupo)
        {
            var estudiantes = await _asistenciaRepo.ListarEstudiantesPorGrupoAsync(idGrupo);
            return estudiantes.Select(e => new RegistroAsistenciaDto
            {
                IdEstudiante = e.IdEstudiante,
                Carnet = e.Carnet,
                NombreCompleto = $"{e.Apellidos}, {e.Nombres}"
            });
        }

        public async Task RegistrarAsistencia(RegistroAsistenciaDto dto) =>
            await _asistenciaRepo.RegistrarAsistenciaAsync(new Asistencia { IdSesion = dto.IdSesion, IdEstudiante = dto.IdEstudiante, EstadoAsistencia = dto.EstadoAsistencia });

        public async Task<IEnumerable<ReporteAsistenciaDto>> GenerarReporte(int idGrupo)
        {
            var list = await _asistenciaRepo.ReporteAsistenciaAsync(idGrupo);
            return list.Select(a => new ReporteAsistenciaDto
            {
                FechaSesion = a.FechaSesion,
                Tema = a.TemaSesion,
                Carnet = a.Carnet,
                Estudiante = a.Estudiante,
                EstadoAsistencia = a.EstadoAsistencia
            });
        }
    }
}
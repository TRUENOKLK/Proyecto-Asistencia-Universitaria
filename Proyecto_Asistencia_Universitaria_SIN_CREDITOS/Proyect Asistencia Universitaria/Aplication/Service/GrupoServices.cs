using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Service
{
    public class GrupoServices
    {
        private readonly IGrupoRepository _repository;
        public GrupoServices(IGrupoRepository repository) => _repository = repository;

        public async Task<IEnumerable<GrupoDto>> ListarGrupos()
        {
            var list = await _repository.ListarTodosLosGruposAsync();
            return list.Select(g => new GrupoDto
            {
                IdGrupo = g.IdGrupo,
                NombreGrupo = g.NombreGrupo,
                NombreAsignatura = g.NombreAsignatura,
                Docente = g.Docente,
                NombrePeriodo = g.NombrePeriodo,
                DiaSemana = g.DiaSemana,
                HoraInicio = g.HoraInicio,
                HoraFin = g.HoraFin
            });
        }

        
        public async Task<IEnumerable<GrupoDto>> ListarGruposPorProfesor(int idProfesor)
        {
            var list = await _repository.ListarGruposPorProfesorAsync(idProfesor);
            return list.Select(g => new GrupoDto
            {
                IdGrupo = g.IdGrupo,
                NombreGrupo = g.NombreGrupo,
                NombreAsignatura = g.NombreAsignatura,
                Docente = g.Docente,
                NombrePeriodo = g.NombrePeriodo,
                DiaSemana = g.DiaSemana,
                HoraInicio = g.HoraInicio,
                HoraFin = g.HoraFin
            });
        }

        public async Task<GrupoDto?> ObtenerPorId(int idGrupo)
        {
            var g = await _repository.ObtenerPorIdAsync(idGrupo);
            if (g == null) return null;
            return new GrupoDto
            {
                IdGrupo = g.IdGrupo,
                NombreGrupo = g.NombreGrupo,
                IdAsignatura = g.IdAsignatura,
                IdProfesor = g.IdProfesor,
                IdPeriodo = g.IdPeriodo,
                NombreAsignatura = g.NombreAsignatura,
                Docente = g.Docente,
                NombrePeriodo = g.NombrePeriodo,
                DiaSemana = g.DiaSemana,
                HoraInicio = g.HoraInicio,
                HoraFin = g.HoraFin
            };
        }

        public async Task NuevoGrupo(GrupoDto dto) =>
            await _repository.InsertarGrupoAsync(new Grupo { NombreGrupo = dto.NombreGrupo, IdAsignatura = dto.IdAsignatura, IdProfesor = dto.IdProfesor, IdPeriodo = dto.IdPeriodo, DiaSemana = dto.DiaSemana, HoraInicio = dto.HoraInicio, HoraFin = dto.HoraFin });

        public async Task EditarGrupo(GrupoDto dto) =>
            await _repository.EditarGrupoAsync(new Grupo { IdGrupo = dto.IdGrupo, NombreGrupo = dto.NombreGrupo, IdAsignatura = dto.IdAsignatura, IdProfesor = dto.IdProfesor, IdPeriodo = dto.IdPeriodo, DiaSemana = dto.DiaSemana, HoraInicio = dto.HoraInicio, HoraFin = dto.HoraFin });

        public async Task EliminarGrupo(int idGrupo) => await _repository.EliminarGrupoAsync(idGrupo);

       

        public async Task<IEnumerable<EstudianteEnGrupoDto>> ListarEstudiantesInscritos(int idGrupo)
        {
            var lista = await _repository.ListarEstudiantesInscritosAsync(idGrupo);
            return lista.Select(e => new EstudianteEnGrupoDto
            {
                IdEstudiante = e.IdEstudiante,
                Carnet = e.Carnet,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                NombreCarrera = e.NombreCarrera,
                Email = e.Email
            });
        }

        public async Task<IEnumerable<EstudianteEnGrupoDto>> ListarEstudiantesDisponibles(int idGrupo)
        {
            var lista = await _repository.ListarEstudiantesDisponiblesAsync(idGrupo);
            return lista.Select(e => new EstudianteEnGrupoDto
            {
                IdEstudiante = e.IdEstudiante,
                Carnet = e.Carnet,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                NombreCarrera = e.NombreCarrera,
                Email = e.Email
            });
        }

        public async Task InscribirEstudiante(int idGrupo, int idEstudiante) =>
            await _repository.InscribirEstudianteAsync(idGrupo, idEstudiante);

        public async Task DesinscribirEstudiante(int idGrupo, int idEstudiante) =>
            await _repository.DesinscribirEstudianteAsync(idGrupo, idEstudiante);
    }
}

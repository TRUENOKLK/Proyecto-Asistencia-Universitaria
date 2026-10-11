using Aplication.Interface;
using Domain.Entities;
using Infrastructure.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Repository
{
    public class GrupoRepository : IGrupoRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public GrupoRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Grupo>> ListarTodosLosGruposAsync()
        {
            var lista = new List<Grupo>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarTodosLosGrupos", con) { CommandType = CommandType.StoredProcedure };
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Grupo
                {
                    IdGrupo = Convert.ToInt32(dr["IdGrupo"]),
                    NombreGrupo = dr["NombreGrupo"].ToString(),
                    NombreAsignatura = dr["NombreAsignatura"].ToString(),
                    Docente = dr["Docente"].ToString(),
                    NombrePeriodo = dr["NombrePeriodo"].ToString(),
                    DiaSemana = dr["DiaSemana"] == DBNull.Value ? null : dr["DiaSemana"].ToString(),
                    HoraInicio = dr["HoraInicio"] == DBNull.Value ? null : (TimeSpan)dr["HoraInicio"],
                    HoraFin = dr["HoraFin"] == DBNull.Value ? null : (TimeSpan)dr["HoraFin"]
                });
            }
            return lista;
        }

        public async Task<IEnumerable<Grupo>> ListarGruposPorProfesorAsync(int idProfesor)
        {
            var lista = new List<Grupo>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarGruposPorProfesor", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdProfesor", idProfesor));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Grupo
                {
                    IdGrupo = Convert.ToInt32(dr["IdGrupo"]),
                    NombreGrupo = dr["NombreGrupo"].ToString(),
                    NombreAsignatura = dr["NombreAsignatura"].ToString(),
                    NombrePeriodo = dr["NombrePeriodo"].ToString(),
                    DiaSemana = dr["DiaSemana"] == DBNull.Value ? null : dr["DiaSemana"].ToString(),
                    HoraInicio = dr["HoraInicio"] == DBNull.Value ? null : (TimeSpan)dr["HoraInicio"],
                    HoraFin = dr["HoraFin"] == DBNull.Value ? null : (TimeSpan)dr["HoraFin"]
                });
            }
            return lista;
        }

        public async Task<Grupo?> ObtenerPorIdAsync(int idGrupo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ObtenerGrupoPorId", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new Grupo
                {
                    IdGrupo = Convert.ToInt32(dr["IdGrupo"]),
                    NombreGrupo = dr["NombreGrupo"].ToString(),
                    IdAsignatura = Convert.ToInt32(dr["IdAsignatura"]),
                    IdProfesor = Convert.ToInt32(dr["IdProfesor"]),
                    IdPeriodo = Convert.ToInt32(dr["IdPeriodo"]),
                    NombreAsignatura = dr["NombreAsignatura"].ToString(),
                    Docente = dr["Docente"].ToString(),
                    NombrePeriodo = dr["NombrePeriodo"].ToString(),
                    DiaSemana = dr["DiaSemana"] == DBNull.Value ? null : dr["DiaSemana"].ToString(),
                    HoraInicio = dr["HoraInicio"] == DBNull.Value ? null : (TimeSpan)dr["HoraInicio"],
                    HoraFin = dr["HoraFin"] == DBNull.Value ? null : (TimeSpan)dr["HoraFin"]
                };
            }
            return null;
        }

        public async Task InsertarGrupoAsync(Grupo oGrupo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            try
            {
                using var cmd = new SqlCommand("SP_InsertarGrupo", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@NombreGrupo", oGrupo.NombreGrupo));
                cmd.Parameters.Add(new SqlParameter("@IdAsignatura", oGrupo.IdAsignatura));
                cmd.Parameters.Add(new SqlParameter("@IdProfesor", oGrupo.IdProfesor));
                cmd.Parameters.Add(new SqlParameter("@IdPeriodo", oGrupo.IdPeriodo));
                cmd.Parameters.Add(new SqlParameter("@DiaSemana", oGrupo.DiaSemana));
                cmd.Parameters.Add(new SqlParameter("@HoraInicio", oGrupo.HoraInicio));
                cmd.Parameters.Add(new SqlParameter("@HoraFin", oGrupo.HoraFin));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task EditarGrupoAsync(Grupo oGrupo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarGrupo", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", oGrupo.IdGrupo));
            cmd.Parameters.Add(new SqlParameter("@NombreGrupo", oGrupo.NombreGrupo));
            cmd.Parameters.Add(new SqlParameter("@IdAsignatura", oGrupo.IdAsignatura));
            cmd.Parameters.Add(new SqlParameter("@IdProfesor", oGrupo.IdProfesor));
            cmd.Parameters.Add(new SqlParameter("@IdPeriodo", oGrupo.IdPeriodo));
            cmd.Parameters.Add(new SqlParameter("@DiaSemana", oGrupo.DiaSemana));
            cmd.Parameters.Add(new SqlParameter("@HoraInicio", oGrupo.HoraInicio));
            cmd.Parameters.Add(new SqlParameter("@HoraFin", oGrupo.HoraFin));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarGrupoAsync(int idGrupo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EliminarGrupo", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<IEnumerable<Estudiante>> ListarEstudiantesInscritosAsync(int idGrupo)
        {
            var lista = new List<Estudiante>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            const string sql = @"
                SELECT e.IdEstudiante, e.Carnet, e.Nombres, e.Apellidos, c.NombreCarrera, u.Email
                FROM Inscripciones i
                INNER JOIN Estudiantes e ON i.IdEstudiante = e.IdEstudiante
                INNER JOIN Carreras c ON e.IdCarrera = c.IdCarrera
                INNER JOIN Usuarios u ON e.IdUsuario = u.IdUsuario
                WHERE i.IdGrupo = @IdGrupo
                ORDER BY e.Apellidos ASC;";
            using var cmd = new SqlCommand(sql, con) { CommandType = CommandType.Text };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Estudiante
                {
                    IdEstudiante = Convert.ToInt32(dr["IdEstudiante"]),
                    Carnet = dr["Carnet"].ToString(),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    NombreCarrera = dr["NombreCarrera"].ToString(),
                    Email = dr["Email"].ToString()
                });
            }
            return lista;
        }

        public async Task<IEnumerable<Estudiante>> ListarEstudiantesDisponiblesAsync(int idGrupo)
        {
            var lista = new List<Estudiante>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
           
            const string sql = @"
                SELECT e.IdEstudiante, e.Carnet, e.Nombres, e.Apellidos, c.NombreCarrera, u.Email
                FROM Estudiantes e
                INNER JOIN Carreras c ON e.IdCarrera = c.IdCarrera
                INNER JOIN Usuarios u ON e.IdUsuario = u.IdUsuario
                WHERE u.Activo = 1
                  AND NOT EXISTS (
                        SELECT 1 FROM Inscripciones i
                        WHERE i.IdGrupo = @IdGrupo AND i.IdEstudiante = e.IdEstudiante
                  )
                ORDER BY e.Apellidos ASC;";
            using var cmd = new SqlCommand(sql, con) { CommandType = CommandType.Text };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Estudiante
                {
                    IdEstudiante = Convert.ToInt32(dr["IdEstudiante"]),
                    Carnet = dr["Carnet"].ToString(),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    NombreCarrera = dr["NombreCarrera"].ToString(),
                    Email = dr["Email"].ToString()
                });
            }
            return lista;
        }

        public async Task InscribirEstudianteAsync(int idGrupo, int idEstudiante)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            const string sql = @"
                IF EXISTS (
                    SELECT 1
                    FROM Inscripciones i
                    INNER JOIN Grupos gOtro ON i.IdGrupo = gOtro.IdGrupo
                    INNER JOIN Grupos gNuevo ON gNuevo.IdGrupo = @IdGrupo
                    WHERE i.IdEstudiante = @IdEstudiante
                      AND i.IdGrupo <> @IdGrupo
                      AND gOtro.DiaSemana = gNuevo.DiaSemana
                      AND gNuevo.HoraInicio IS NOT NULL AND gNuevo.HoraFin IS NOT NULL
                      AND gOtro.HoraInicio IS NOT NULL AND gOtro.HoraFin IS NOT NULL
                      AND gNuevo.HoraInicio < gOtro.HoraFin AND gNuevo.HoraFin > gOtro.HoraInicio
                )
                BEGIN
                    THROW 51007, 'EL ESTUDIANTE YA TIENE OTRA CLASE EN ESE MISMO DIA Y HORARIO', 1;
                END

                IF NOT EXISTS (SELECT 1 FROM Inscripciones WHERE IdGrupo = @IdGrupo AND IdEstudiante = @IdEstudiante)
                INSERT INTO Inscripciones (IdEstudiante, IdGrupo) VALUES (@IdEstudiante, @IdGrupo);";
            using var cmd = new SqlCommand(sql, con) { CommandType = CommandType.Text };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            cmd.Parameters.Add(new SqlParameter("@IdEstudiante", idEstudiante));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DesinscribirEstudianteAsync(int idGrupo, int idEstudiante)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            const string sql = "DELETE FROM Inscripciones WHERE IdGrupo = @IdGrupo AND IdEstudiante = @IdEstudiante;";
            using var cmd = new SqlCommand(sql, con) { CommandType = CommandType.Text };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            cmd.Parameters.Add(new SqlParameter("@IdEstudiante", idEstudiante));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
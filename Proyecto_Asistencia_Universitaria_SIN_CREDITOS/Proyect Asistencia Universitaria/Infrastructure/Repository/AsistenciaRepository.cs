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
    public class AsistenciaRepository : IAsistenciaRepository
    {
        private readonly DBConnectionFactory _connectionFactory;
        public AsistenciaRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Estudiante>> ListarEstudiantesPorGrupoAsync(int idGrupo)
        {
            var lista = new List<Estudiante>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarEstudiantesPorGrupo", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Estudiante
                {
                    IdEstudiante = Convert.ToInt32(dr["IdEstudiante"]),
                    Carnet = dr["Carnet"].ToString(),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString()
                });
            }
            return lista;
        }

        public async Task RegistrarAsistenciaAsync(Asistencia asistencia)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            try
            {
                using var cmd = new SqlCommand("SP_RegistrarAsistencia", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdSesion", asistencia.IdSesion));
                cmd.Parameters.Add(new SqlParameter("@IdEstudiante", asistencia.IdEstudiante));
                cmd.Parameters.Add(new SqlParameter("@EstadoAsistencia", asistencia.EstadoAsistencia));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public async Task<IEnumerable<Asistencia>> ReporteAsistenciaAsync(int idGrupo)
        {
            var lista = new List<Asistencia>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ReporteAsistencia", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Asistencia
                {
                    FechaSesion = Convert.ToDateTime(dr["FechaSesion"]),
                    TemaSesion = dr["Tema"].ToString(),
                    Carnet = dr["Carnet"].ToString(),
                    Estudiante = dr["Estudiante"].ToString(),
                    EstadoAsistencia = dr["EstadoAsistencia"].ToString()
                });
            }
            return lista;
        }
    }
}
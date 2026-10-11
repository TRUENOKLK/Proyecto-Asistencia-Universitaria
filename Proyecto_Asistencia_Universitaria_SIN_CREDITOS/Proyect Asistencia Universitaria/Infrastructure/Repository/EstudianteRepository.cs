using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Repository
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public EstudianteRepository(DBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Estudiante>> ListarEstudiantesAsync()
        {
            var olist = new List<Estudiante>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("SP_ListarEstudiantes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            using var Dr = await cmd.ExecuteReaderAsync();
            while (await Dr.ReadAsync())
            {
                var oEstudiante = new Estudiante
                {
                    IdEstudiante = Convert.ToInt32(Dr["IdEstudiante"]),
                    Carnet = Dr["Carnet"].ToString(),
                    Nombres = Dr["Nombres"].ToString(),
                    Apellidos = Dr["Apellidos"].ToString(),
                    NombreCarrera = Dr["NombreCarrera"].ToString(),
                    Email = Dr["Email"].ToString(),
                    Activo = Convert.ToBoolean(Dr["Activo"])
                };
                olist.Add(oEstudiante);
            }
            return olist;
        }

        public async Task<Estudiante?> ObtenerPorIdAsync(int idEstudiante)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ObtenerEstudiantePorId", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdEstudiante", idEstudiante));
            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new Estudiante
                {
                    IdEstudiante = Convert.ToInt32(dr["IdEstudiante"]),
                    Carnet = dr["Carnet"].ToString(),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    IdCarrera = Convert.ToInt32(dr["IdCarrera"]),
                    NombreCarrera = dr["NombreCarrera"].ToString(),
                    Email = dr["Email"].ToString(),
                    Activo = Convert.ToBoolean(dr["Activo"])
                };
            }
            return null;
        }

        public async Task EditarEstudianteAsync(Estudiante oEstudiante)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarEstudiante", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdEstudiante", oEstudiante.IdEstudiante));
            cmd.Parameters.Add(new SqlParameter("@Nombres", oEstudiante.Nombres));
            cmd.Parameters.Add(new SqlParameter("@Apellidos", oEstudiante.Apellidos));
            cmd.Parameters.Add(new SqlParameter("@IdCarrera", oEstudiante.IdCarrera));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task CambiarEstadoAsync(int idEstudiante, bool activo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_CambiarEstadoEstudiante", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdEstudiante", idEstudiante));
            cmd.Parameters.Add(new SqlParameter("@Activo", activo));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task InsertarEstudianteAsync(Estudiante oEstudiante)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            try
            {
                using var cmd = new SqlCommand("SP_InsertarEstudiante", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@Email", oEstudiante.Email ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@PasswordHash", oEstudiante.PasswordHash ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Carnet", oEstudiante.Carnet));
                cmd.Parameters.Add(new SqlParameter("@Nombres", oEstudiante.Nombres));
                cmd.Parameters.Add(new SqlParameter("@Apellidos", oEstudiante.Apellidos));
                cmd.Parameters.Add(new SqlParameter("@IdCarrera", oEstudiante.IdCarrera));

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InsertarMasivoAsync(IEnumerable<Estudiante> listaEstudiantes)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

          
            using var transaction = con.BeginTransaction();

            try
            {
                foreach (var oEstudiante in listaEstudiantes)
                {
                    using var cmd = new SqlCommand("SP_InsertarEstudiante", con, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Email", (object)oEstudiante.Email ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@PasswordHash", (object)oEstudiante.PasswordHash ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Carnet", oEstudiante.Carnet));
                    cmd.Parameters.Add(new SqlParameter("@Nombres", oEstudiante.Nombres));
                    cmd.Parameters.Add(new SqlParameter("@Apellidos", oEstudiante.Apellidos));
                    cmd.Parameters.Add(new SqlParameter("@IdCarrera", oEstudiante.IdCarrera));

                    await cmd.ExecuteNonQueryAsync();
                }

                
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                throw new Exception("Error en el registro masivo de estudiantes: " + ex.Message, ex);
            }
        }

        
        public async Task<IEnumerable<MiAsistenciaDto>> ObtenerAsistenciaPorEmailAsync(string email)
        {
            var olist = new List<MiAsistenciaDto>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("SP_ObtenerAsistenciaPorEstudianteEmail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Email", email));

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                olist.Add(new MiAsistenciaDto
                {
                    FechaSesion = Convert.ToDateTime(dr["FechaSesion"]),
                    Tema = dr["Tema"].ToString(),
                    NombreAsignatura = dr["NombreAsignatura"].ToString(),
                    NombreGrupo = dr["NombreGrupo"].ToString(),
                    EstadoAsistencia = dr["EstadoAsistencia"].ToString()
                });
            }
            return olist;
        }
    }
}
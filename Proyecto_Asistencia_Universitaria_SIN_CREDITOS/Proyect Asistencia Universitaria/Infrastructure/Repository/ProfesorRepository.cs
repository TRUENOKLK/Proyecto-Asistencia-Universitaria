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
    public class ProfesorRepository : IProfesorRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public ProfesorRepository(DBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Profesor>> ListarProfesoresAsync()
        {
            var lista = new List<Profesor>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("SP_ListarProfesores", con);
            cmd.CommandType = CommandType.StoredProcedure;

            using var Dr = await cmd.ExecuteReaderAsync();
            while (await Dr.ReadAsync())
            {
                lista.Add(new Profesor
                {
                    IdProfesor = Convert.ToInt32(Dr["IdProfesor"]),
                    Nombres = Dr["Nombres"].ToString(),
                    Apellidos = Dr["Apellidos"].ToString(),
                    Telefono = Dr["Telefono"] != DBNull.Value ? Dr["Telefono"].ToString() : null,
                    Email = Dr["Email"].ToString(),
                    Activo = Convert.ToBoolean(Dr["Activo"])
                });
            }
            return lista;
        }

        public async Task<Profesor?> ObtenerPorIdAsync(int idProfesor)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ObtenerProfesorPorId", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdProfesor", idProfesor));
            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new Profesor
                {
                    IdProfesor = Convert.ToInt32(dr["IdProfesor"]),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : null,
                    Email = dr["Email"].ToString(),
                    Activo = Convert.ToBoolean(dr["Activo"])
                };
            }
            return null;
        }

        public async Task EditarProfesorAsync(Profesor oProfesor)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarProfesor", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdProfesor", oProfesor.IdProfesor));
            cmd.Parameters.Add(new SqlParameter("@Nombres", oProfesor.Nombres));
            cmd.Parameters.Add(new SqlParameter("@Apellidos", oProfesor.Apellidos));
            cmd.Parameters.Add(new SqlParameter("@Telefono", (object?)oProfesor.Telefono ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task CambiarEstadoAsync(int idProfesor, bool activo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_CambiarEstadoProfesor", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdProfesor", idProfesor));
            cmd.Parameters.Add(new SqlParameter("@Activo", activo));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task InsertarProfesorAsync(Profesor oProfesor)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            try
            {
                using var cmd = new SqlCommand("SP_InsertarProfesor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@Email", oProfesor.Email));
                cmd.Parameters.Add(new SqlParameter("@PasswordHash", oProfesor.PasswordHash));
                cmd.Parameters.Add(new SqlParameter("@Nombres", oProfesor.Nombres));
                cmd.Parameters.Add(new SqlParameter("@Apellidos", oProfesor.Apellidos));
                cmd.Parameters.Add(new SqlParameter("@Telefono", oProfesor.Telefono ?? (object)DBNull.Value));

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }
    }
}
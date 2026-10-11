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
    public class AsignaturaRepository : IAsignaturaRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public AsignaturaRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Asignatura>> ListarAsignaturasAsync()
        {
            var lista = new List<Asignatura>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarAsignaturas", con) { CommandType = CommandType.StoredProcedure };
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Asignatura
                {
                    IdAsignatura = Convert.ToInt32(dr["IdAsignatura"]),
                    Codigo = dr["Codigo"].ToString(),
                    NombreAsignatura = dr["NombreAsignatura"].ToString(),
                    Creditos = Convert.ToInt32(dr["Creditos"]),
                    IdCarrera = Convert.ToInt32(dr["IdCarrera"]),
                    NombreCarrera = dr["NombreCarrera"].ToString()
                });
            }
            return lista;
        }

        public async Task InsertarAsignaturaAsync(Asignatura oAsignatura)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            try
            {
                using var cmd = new SqlCommand("SP_InsertarAsignatura", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Codigo", oAsignatura.Codigo));
                cmd.Parameters.Add(new SqlParameter("@NombreAsignatura", oAsignatura.NombreAsignatura));
                cmd.Parameters.Add(new SqlParameter("@Creditos", oAsignatura.Creditos));
                cmd.Parameters.Add(new SqlParameter("@IdCarrera", oAsignatura.IdCarrera));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }
        public async Task EditarAsignaturaAsync(Asignatura oAsignatura)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarAsignatura", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdAsignatura", oAsignatura.IdAsignatura));
            cmd.Parameters.Add(new SqlParameter("@Codigo", oAsignatura.Codigo));
            cmd.Parameters.Add(new SqlParameter("@NombreAsignatura", oAsignatura.NombreAsignatura));
            cmd.Parameters.Add(new SqlParameter("@Creditos", oAsignatura.Creditos));
            cmd.Parameters.Add(new SqlParameter("@IdCarrera", oAsignatura.IdCarrera));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsignaturaAsync(int idAsignatura)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EliminarAsignatura", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdAsignatura", idAsignatura));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
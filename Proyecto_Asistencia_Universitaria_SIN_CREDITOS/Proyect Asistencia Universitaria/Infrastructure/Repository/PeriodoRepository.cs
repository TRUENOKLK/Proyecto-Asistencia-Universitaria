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

    public class PeriodoRepository : IPeriodoRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public PeriodoRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Periodo>> ListarPeriodosAsync()
        {
            var lista = new List<Periodo>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarPeriodos", con) { CommandType = CommandType.StoredProcedure };
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Periodo
                {
                    IdPeriodo = Convert.ToInt32(dr["IdPeriodo"]),
                    NombrePeriodo = dr["NombrePeriodo"].ToString(),
                    FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                    FechaFin = Convert.ToDateTime(dr["FechaFin"])
                });
            }
            return lista;
        }

        public async Task InsertarPeriodoAsync(Periodo oPeriodo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            try
            {
                using var cmd = new SqlCommand("SP_InsertarPeriodo", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@NombrePeriodo", oPeriodo.NombrePeriodo));
                cmd.Parameters.Add(new SqlParameter("@FechaInicio", oPeriodo.FechaInicio));
                cmd.Parameters.Add(new SqlParameter("@FechaFin", oPeriodo.FechaFin));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }
        public async Task EditarPeriodoAsync(Periodo oPeriodo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarPeriodo", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdPeriodo", oPeriodo.IdPeriodo));
            cmd.Parameters.Add(new SqlParameter("@NombrePeriodo", oPeriodo.NombrePeriodo));
            cmd.Parameters.Add(new SqlParameter("@FechaInicio", oPeriodo.FechaInicio));
            cmd.Parameters.Add(new SqlParameter("@FechaFin", oPeriodo.FechaFin));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarPeriodoAsync(int idPeriodo)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EliminarPeriodo", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdPeriodo", idPeriodo));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}

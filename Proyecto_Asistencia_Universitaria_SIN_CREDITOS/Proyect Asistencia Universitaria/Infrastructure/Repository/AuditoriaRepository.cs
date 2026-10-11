using Aplication.Interface;
using Domain.Entities;
using Infrastructure.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly DBConnectionFactory _connectionFactory;
        public AuditoriaRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task RegistrarAsync(string email, string accion, string modulo, string? procedimiento, string? detalle)
        {
            try
            {
                using var con = _connectionFactory.CreateConnection();
                await con.OpenAsync();
                using var cmd = new SqlCommand("SP_RegistrarAuditoria", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Email", string.IsNullOrWhiteSpace(email) ? "desconocido" : email));
                cmd.Parameters.Add(new SqlParameter("@Accion", accion));
                cmd.Parameters.Add(new SqlParameter("@Modulo", modulo));
                cmd.Parameters.Add(new SqlParameter("@Procedimiento", (object?)procedimiento ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Detalle", (object?)detalle ?? DBNull.Value));
                await cmd.ExecuteNonQueryAsync();
            }
            catch
            {
                
            }
        }

        public async Task<IEnumerable<RegistroAuditoria>> ListarAsync(string? accion)
        {
            var lista = new List<RegistroAuditoria>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarAuditoria", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Accion", (object?)accion ?? DBNull.Value));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new RegistroAuditoria
                {
                    IdAuditoria = Convert.ToInt32(dr["IdAuditoria"]),
                    Email = dr["Email"].ToString(),
                    Accion = dr["Accion"].ToString(),
                    Modulo = dr["Modulo"].ToString(),
                    Procedimiento = dr["Procedimiento"] != DBNull.Value ? dr["Procedimiento"].ToString() : null,
                    Detalle = dr["Detalle"] != DBNull.Value ? dr["Detalle"].ToString() : null,
                    FechaHora = Convert.ToDateTime(dr["FechaHora"])
                });
            }
            return lista;
        }

        public async Task<int> ObtenerContadorSesionesAsync()
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ObtenerContadorSesiones", con) { CommandType = CommandType.StoredProcedure };
            var resultado = await cmd.ExecuteScalarAsync();
            return resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
        }
    }
}

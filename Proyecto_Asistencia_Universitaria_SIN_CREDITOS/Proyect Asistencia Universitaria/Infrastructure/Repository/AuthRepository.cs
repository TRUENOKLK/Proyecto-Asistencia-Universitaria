using Aplication.Interface;
using Domain.Entities;
using Infrastructure.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DBConnectionFactory _connectionFactory;
        public AuthRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ObtenerUsuarioPorEmail", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Email", email));

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new Usuario
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    Email = dr["Email"].ToString(),
                    PasswordHash = dr["PasswordHash"].ToString(),
                    NombreRol = dr["NombreRol"].ToString(),
                    Activo = Convert.ToBoolean(dr["Activo"])
                };
            }
            return null; 
        }
    }
}

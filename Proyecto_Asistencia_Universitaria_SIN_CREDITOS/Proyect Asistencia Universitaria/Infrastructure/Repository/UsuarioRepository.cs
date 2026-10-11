using Aplication.Interface;
using Infrastructure.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DBConnectionFactory _connectionFactory;
        public UsuarioRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<bool> ExisteEmailAsync(string email)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("SP_ExisteEmail", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Email", email));

            var resultado = await cmd.ExecuteScalarAsync();
            return resultado != null && Convert.ToBoolean(resultado);
        }
    }
}

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
    public class SesionRepository : ISesionRepository
    {
        private readonly DBConnectionFactory _connectionFactory;
        public SesionRepository(DBConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task CrearSesionAsync(Sesion sesion)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            try
            {
                using var cmd = new SqlCommand("SP_CrearSesion", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdGrupo", sesion.IdGrupo));
                cmd.Parameters.Add(new SqlParameter("@FechaSesion", sesion.FechaSesion));
                cmd.Parameters.Add(new SqlParameter("@Tema", sesion.Tema));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public async Task<IEnumerable<Sesion>> ListarSesionesPorGrupoAsync(int idGrupo)
        {
            var lista = new List<Sesion>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarSesionesPorGrupo", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdGrupo", idGrupo));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Sesion
                {
                    IdSesion = Convert.ToInt32(dr["IdSesion"]),
                    FechaSesion = Convert.ToDateTime(dr["FechaSesion"]),
                    Tema = dr["Tema"].ToString()
                });
            }
            return lista;
        }
    }
}
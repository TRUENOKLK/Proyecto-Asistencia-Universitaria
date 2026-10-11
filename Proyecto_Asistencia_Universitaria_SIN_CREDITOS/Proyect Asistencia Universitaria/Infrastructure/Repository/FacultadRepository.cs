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
    public class FacultadRepository : IFacultadRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public FacultadRepository(DBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Facultad>> ListarFacultadesAsync()
        {
            var lista = new List<Facultad>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_ListarFacultades", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using var Dr = await cmd.ExecuteReaderAsync();
            while (await Dr.ReadAsync())
            {
                lista.Add(new Facultad
                {
                    IdFacultad = Convert.ToInt32(Dr["IdFacultad"]),
                    NombreFacultad = Dr["NombreFacultad"].ToString()
                });
            }
            return lista;
        }

        public async Task InsertarFacultadAsync(Facultad oFacultad)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            try
            {
                using var cmd = new SqlCommand("SP_InsertarFacultad", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreFacultad", oFacultad.NombreFacultad));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task EditarFacultadAsync(Facultad oFacultad)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarFacultad", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdFacultad", oFacultad.IdFacultad));
            cmd.Parameters.Add(new SqlParameter("@NombreFacultad", oFacultad.NombreFacultad));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarFacultadAsync(int idFacultad)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EliminarFacultad", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdFacultad", idFacultad));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
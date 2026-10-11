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
    public class CarreraRepository : ICarreraRepository
    {
        private readonly DBConnectionFactory _connectionFactory;

        public CarreraRepository(DBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Carrera>> ListarCarrerasAsync()
        {
            var lista = new List<Carrera>();
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("SP_ListarCarreras", con);
            cmd.CommandType = CommandType.StoredProcedure;

            using var Dr = await cmd.ExecuteReaderAsync();
            while (await Dr.ReadAsync())
            {
                lista.Add(new Carrera
                {
                    IdCarrera = Convert.ToInt32(Dr["IdCarrera"]),
                    NombreCarrera = Dr["NombreCarrera"].ToString(),
                    IdFacultad = Convert.ToInt32(Dr["IdFacultad"]),
                    NombreFacultad = Dr["NombreFacultad"].ToString()
                });
            }
            return lista;
        }

        public async Task InsertarCarreraAsync(Carrera oCarrera)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();

            try
            {
                using var cmd = new SqlCommand("SP_InsertarCarrera", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NombreCarrera", oCarrera.NombreCarrera));
                cmd.Parameters.Add(new SqlParameter("@IdFacultad", oCarrera.IdFacultad));

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }
        public async Task EditarCarreraAsync(Carrera oCarrera)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EditarCarrera", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdCarrera", oCarrera.IdCarrera));
            cmd.Parameters.Add(new SqlParameter("@NombreCarrera", oCarrera.NombreCarrera));
            cmd.Parameters.Add(new SqlParameter("@IdFacultad", oCarrera.IdFacultad));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarCarreraAsync(int idCarrera)
        {
            using var con = _connectionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("SP_EliminarCarrera", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IdCarrera", idCarrera));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DataBase
{
    public class DBConnectionFactory
    {
        private readonly string _connectionString;

        public DBConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {

            return new SqlConnection(_connectionString);
        }
    }
}

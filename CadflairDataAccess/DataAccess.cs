using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CadflairDataAccess
{
    public class DataAccess
    {

        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var rows = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return rows.ToList();
            }
        }

        public async Task<T> LoadSingleAsync<T, U>(string storedProcedure, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var row = await connection.QueryFirstOrDefaultAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return row;
            }
        }

        public async Task SaveDataAsync<T>(string storedProcedure, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> SaveSingleAsync<T>(string storedProcedure, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                int id = await connection.QuerySingleAsync<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return id;
            }
        }

    }
}

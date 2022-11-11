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
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _config;

        public string ConnectionStringName { get; set; } = "Development";

        public DataAccess(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(ConnectionStringName)))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);
                return rows.ToList();
            }
        }

        public async Task<T> LoadSingleAsync<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(ConnectionStringName)))
            {
                var row = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
                return row;
            }
        }

        public async Task SaveDataAsync<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(ConnectionStringName)))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }

    }
}

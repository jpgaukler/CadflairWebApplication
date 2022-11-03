using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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

        public List<T> LoadData<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(ConnectionStringName)))
            {
                List<T> rows = connection.Query<T>(sql, parameters).ToList();
                return rows;
            }
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(ConnectionStringName)))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);
                return rows.ToList();
            }
        }

        public void SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(ConnectionStringName)))
            {
                connection.Execute(sql, parameters);
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

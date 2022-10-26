using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadflairDataAccess
{
    public interface IDataAccess
    {
        List<T> LoadData<T, U>(string sql, U parameters, string connectionString);
        Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters, string connectionString);
        void SaveData<T>(string sql, T parameters, string connectionString);
        Task SaveDataAsync<T>(string sql, T parameters, string connectionString);
    }
}
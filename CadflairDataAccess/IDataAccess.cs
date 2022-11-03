using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadflairDataAccess
{
    public interface IDataAccess
    {
        string ConnectionStringName { get; set; }

        List<T> LoadData<T, U>(string sql, U parameters);
        Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters);
        void SaveData<T>(string sql, T parameters);
        Task SaveDataAsync<T>(string sql, T parameters);
    }
}
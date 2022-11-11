using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadflairDataAccess
{
    public interface IDataAccess
    {
        string ConnectionStringName { get; set; }

        Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters);
        Task<T> LoadSingleAsync<T, U>(string sql, U parameters);
        Task SaveDataAsync<T>(string sql, T parameters);
    }
}
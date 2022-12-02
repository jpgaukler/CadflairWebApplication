using CadflairDataAccess.Services;
using Microsoft.Extensions.Configuration;

namespace CadflairDataAccess
{
    public class DataServicesManager
    {
        private readonly string _connectionString;
        private readonly DataAccess _dataAccess;

        public AccountService AccountService { get; }
        public UserService UserService { get; }
        public ProductService ProductService { get; }

        public DataServicesManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
            _dataAccess = new DataAccess(_connectionString);
            AccountService = new AccountService(_dataAccess);
            UserService = new UserService(_dataAccess);
            ProductService = new ProductService(_dataAccess);
        }
    }
}

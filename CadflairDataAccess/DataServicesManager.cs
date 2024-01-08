using CadflairDataAccess.Services;
using Microsoft.Extensions.Configuration;

namespace CadflairDataAccess
{
    public class DataServicesManager
    {
        private readonly string _connectionString;
        private readonly DataAccess _dataAccess;

        public SubscriptionService SubscriptionService { get; }
        public UserService UserService { get; }
        public ProductService ProductService { get; }
        public CadDriveService CadDriveService { get; }
        public NotificationService NotificationService { get; }
        public McMasterService McMasterService { get; }

        public DataServicesManager(string connectionString)
        {
            _connectionString = connectionString;
            _dataAccess = new DataAccess(_connectionString);
            SubscriptionService = new SubscriptionService(_dataAccess);
            NotificationService = new NotificationService(_dataAccess);
            UserService = new UserService(_dataAccess, NotificationService);
            ProductService = new ProductService(_dataAccess);
            CadDriveService = new CadDriveService(_dataAccess);
            McMasterService = new McMasterService(_dataAccess);
        }

        public DataServicesManager(IConfiguration configuration) : this(configuration.GetConnectionString("Default"))
        {
        }
    }
}

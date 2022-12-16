using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class SubscriptionService
    {

        private readonly DataAccess _db;

        public SubscriptionService(DataAccess db)
        {
            _db = db;
        }

        public Task<List<SubscriptionType>> GetSubscriptionTypes()
        {
            return _db.LoadDataAsync<SubscriptionType, dynamic>("[dbo].[spSubscriptionType_GetAll]", new { });
        }

        public Task<Subscription> GetSubscriptionById(int id)
        {
            return _db.LoadSingleAsync<Subscription, dynamic>("[dbo].[spSubscription_GetById]", new { Id = id });
        }

        public Task<Subscription> CreateSubscription(int subscriptionTypeId, string companyName, int ownerId, int createdById)
        {
            dynamic values = new
            {
                SubscriptionTypeId = subscriptionTypeId,
                CompanyName = companyName,
                SubdirectoryName =  Regex.Replace(companyName, "[^a-zA-Z0-9_.]+", string.Empty).ToLower(),
                OwnerId = ownerId,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<Subscription, dynamic>("[dbo].[spSubscription_Insert]", values);
        }

        public Task DeleteSubscription(Subscription subscription)
        {
            return _db.SaveDataAsync("[dbo].[spSubscription_DeleteById]", new { subscription.Id });
        }
    }
}

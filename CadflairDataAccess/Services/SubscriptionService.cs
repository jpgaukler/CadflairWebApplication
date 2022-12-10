using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //public Task<List<Subscription>> GetSubscriptions()
        //{
        //    string sql = "select * from dbo.Subscription";

        //    return _db.LoadDataAsync<Subscription, dynamic>(sql, new { });
        //}

        //public Task<List<SubscriptionType>> GetSubscriptionTypes()
        //{
        //    string sql = "select * from dbo.SubscriptionType";

        //    return _db.LoadDataAsync<SubscriptionType, dynamic>(sql, new { });
        //}

        public async Task<Subscription> GetSubscriptionById(int subscriptionId)
        {
            string sql = "select * from dbo.Subscription where Id = @Id";

            dynamic values = new
            {
                Id = subscriptionId,
            };

            List<Subscription> accounts = await _db.LoadDataAsync<Subscription, dynamic>(sql, values);

            return accounts.First();
        }

        public async Task<Subscription> GetSubscriptionByPageName(string pageName)
        {
            string sql = "select * from dbo.Subscription where PageName = @PageName";

            dynamic values = new
            {
                PageName = pageName,
            };

            List<Subscription> accounts = await _db.LoadDataAsync<Subscription, dynamic>(sql, values);

            return accounts.First();
        }

        //public Task CreateAccount(Subscription newAccount)
        //{
        //    string sql = @"INSERT INTO [dbo].[Account]
        //                   (CompanyName
        //                   ,SubDirectory
        //                   ,CreatedById
        //                   ,OwnerId
        //                   ,AccountTypeId
        //                   ,SubscriptionExpiresOn)
        //                   VALUES
        //                   (@CompanyName
        //                   ,@SubDirectory
        //                   ,@CreatedById
        //                   ,@OwnerId
        //                   ,@AccountTypeId
        //                   ,@SubscriptionExpiresOn)";

        //    return _db.SaveDataAsync(sql, newAccount);
        //}

        public Task DeleteSubscription(Subscription subscription)
        {
            string sql = "DELETE FROM [dbo].[Subscription] WHERE Id = @Id";
            return _db.SaveDataAsync(sql, new { subscription.Id });
        }
    }
}

﻿using CadflairDataAccess.Models;
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

        public Task<List<SubscriptionType>> GetSubscriptionTypes()
        {
            return _db.LoadDataAsync<SubscriptionType, dynamic>("[dbo].[spSubscriptionType_GetAll]", new { });
        }

        public Task<Subscription> GetSubscriptionById(int id)
        {
            return _db.LoadSingleAsync<Subscription, dynamic>("[dbo].[spSubscription_GetById]", new { Id = id });
        }

        //public async Task<Subscription> GetSubscriptionByPageName(string pageName)
        //{
        //    string sql = "select * from dbo.Subscription where PageName = @PageName";

        //    dynamic values = new
        //    {
        //        PageName = pageName,
        //    };

        //    List<Subscription> accounts = await _db.LoadDataAsync<Subscription, dynamic>(sql, values);

        //    return accounts.First();
        //}

        public Task<Subscription> CreateSubscription(int subscriptionTypeId, string companyName, string pageName, int ownerId, int createdById)
        {
            dynamic values = new
            {
                SubscriptionTypeId = subscriptionTypeId,
                CompanyName = companyName,
                PageName = pageName,
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

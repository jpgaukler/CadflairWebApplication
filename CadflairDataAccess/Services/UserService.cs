using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class UserService
    {

        private readonly DataAccess _db;
        private readonly NotificationService _notificationService;

        public UserService(DataAccess db, NotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        #region "Users"

        public async Task<User> CreateUser(Guid objectIdentifier, string firstName, string lastName, string emailAddress)
        {
            dynamic values = new
            {
                ObjectIdentifier = objectIdentifier,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
            };

            User newUser = await _db.SaveSingleAsync<User, dynamic>("[dbo].[spUser_Insert]", values);

            // set up default notifications settings
            foreach (Notification notification in await _notificationService.GetNotifications())
                await _notificationService.CreateNotificationSetting(notification.Id, newUser.Id);

            return newUser;
        }

        public Task<User> GetUserByObjectIdentifier(string objectIdentifier)
        {
            return _db.LoadSingleAsync<User, dynamic>("[dbo].[spUser_GetByObjectIdentifier]", new { ObjectIdentifier = objectIdentifier });
        }

        public Task<List<User>> GetUsersBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<User, dynamic>("[dbo].[spUser_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }

        public Task UpdateUser(User user)
        {
            dynamic values = new
            {
                user.Id,
                user.ObjectIdentifier,
                user.SubscriptionId,
                user.FirstName,
                user.LastName,
                user.EmailAddress,
            };

            return _db.SaveDataAsync("[dbo].[spUser_UpdateById]", values);
        }

        public Task DeleteUserById(int id)
        {
            return _db.SaveDataAsync("[dbo].[spUser_DeleteById]", new { Id = id });
        }

        #endregion

    }
}

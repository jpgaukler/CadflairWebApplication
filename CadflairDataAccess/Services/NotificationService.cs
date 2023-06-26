using CadflairDataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class NotificationService
    {

        private readonly DataAccess _db;

        public NotificationService(DataAccess db)
        {
            _db = db;
        }

        #region "Notification"

        public Task<List<Notification>> GetNotifications()
        {
            return _db.LoadDataAsync<Notification, dynamic>("[dbo].[spNotification_GetAll]", new { });
        }

        public Task<Notification> GetNotificationById(int id)
        {
            return _db.LoadSingleAsync<Notification, dynamic>("[dbo].[spNotification_GetById]", new { Id = id });
        }

        public Task<Notification> GetNotificationByEventName(string eventName)
        {
            return _db.LoadSingleAsync<Notification, dynamic>("[dbo].[spNotification_GetByEventName]", new { EventName = eventName });
        }

        #endregion

        #region "NotificationSetting"

        public async Task<NotificationSetting> CreateNotificationSetting(int notificationId, int userId)
        {
            Notification notification = await GetNotificationById(notificationId);

            dynamic values = new
            {
                NotificationId = notification.Id,
                UserId = userId,
                IsEnabled = notification.EnabledByDefault
            };

            NotificationSetting newSetting = await _db.SaveSingleAsync<NotificationSetting, dynamic>("[dbo].[spNotificationSetting_Insert]", values);
            return newSetting; 
        }

        public Task<List<NotificationSetting>> GetNotificationSettingsByUserId(int userId)
        {
            return _db.LoadDataAsync<NotificationSetting, dynamic>("[dbo].[spNotificationSetting_GetByUserId]", new { UserId = userId });
        }
      
        #endregion

        public async Task<ContactRequest> CreateContactRequest(string firtName, string lastName, string emailAddress, string companyName, string message)
        {
            dynamic values = new
            {
                FirstName = firtName,
                LastName = lastName,
                EmailAddress = emailAddress,
                CompanyName = companyName,
                Message = message,
            };

            ContactRequest newSetting = await _db.SaveSingleAsync<ContactRequest, dynamic>("[dbo].[spContactRequest_Insert]", values);
            return newSetting; 
        }

    }
}

using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<Notification> GetNotificationByEventName(string eventName)
        {
            return _db.LoadSingleAsync<Notification, dynamic>("[dbo].[spNotification_GetByEventName]", new { EventName = eventName });
        }

        #endregion

        #region "NotificationSetting"

        public Task<NotificationSetting> CreateNotificationSetting(int notificationId, int userId, bool isEnabled)
        {
            dynamic values = new
            {
                NotificationId = notificationId,
                UserId = userId,
                IsEnabled = isEnabled
            };

            return _db.SaveSingleAsync<NotificationSetting, dynamic>("[dbo].[spNotificationSetting_Insert]", values);
        }

        public Task<List<NotificationSetting>> GetNotificationSettingsByUserId(int userId)
        {
            return _db.LoadDataAsync<NotificationSetting, dynamic>("[dbo].[spNotificationSetting_GetByUserId]", new { UserId = userId });
        }

        public Task<NotificationSetting> GetNotificationSettingByNotificationIdAndUserId(int notificationId, int userId)
        {
            dynamic values = new
            {
                NotificationId = notificationId,
                UserId = userId,
            };

            return _db.LoadSingleAsync<NotificationSetting, dynamic>("[dbo].[spNotificationSetting_GetByNotificationIdAndUserId]", values);
        }

        #endregion

    }
}

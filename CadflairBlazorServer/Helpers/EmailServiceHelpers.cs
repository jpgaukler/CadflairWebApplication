using CadflairDataAccess;
using CadflairDataAccess.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using System.Diagnostics;

namespace CadflairBlazorServer.Helpers
{
    public static class EmailServiceHelpers
    {
        private static string _templateRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates");

        public static async Task SendNotificationEmail<T>(this IFluentEmail emailService, DataServicesManager dataServicesManager, int subscriptionId, string eventName, string subject, string templateFilename, T model)
        {
            // get notification
            Notification notification = await dataServicesManager.NotificationService.GetNotificationByEventName(eventName);

            // get users that subscribe to notification
            List<Address> addresses = new();
            List<User> users = await dataServicesManager.UserService.GetUsersBySubscriptionId(subscriptionId);

            foreach (User user in users)
            {
                NotificationSetting setting = await dataServicesManager.NotificationService.GetNotificationSettingByNotificationIdAndUserId(notification.Id, user.Id);
                if (setting == null || !setting.IsEnabled) continue;

                SendResponse email = await emailService.SetFrom("donotreply@cadflair.com")
                                                       .To(user.EmailAddress)
                                                       .Subject(subject)
                                                       .UsingTemplateFromFile(Path.Combine(_templateRoot, templateFilename), model)
                                                       .SendAsync();

                Trace.WriteLine($"Notification email sent. Event name: {eventName} Success: {email.Successful}");

                if (!email.Successful)
                {
                    email.ErrorMessages.ForEach(message => Trace.TraceError($"Failed to send email: {message}"));
                }
            }
        }
    }
}

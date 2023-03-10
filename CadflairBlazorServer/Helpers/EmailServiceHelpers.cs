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

            foreach (User user in await dataServicesManager.UserService.GetUsersBySubscriptionId(subscriptionId))
            {
                NotificationSetting setting = await dataServicesManager.NotificationService.GetNotificationSettingByNotificationIdAndUserId(notification.Id, user.Id);
                if (setting == null || !setting.IsEnabled) continue;

                SendResponse email = await emailService.SetFrom("donotreply@cadflair.com")
                                                       .To(user.EmailAddress)
                                                       .Subject(subject)
                                                       .UsingTemplateFromFile(Path.Combine(_templateRoot, templateFilename), model)
                                                       .SendAsync();

                Trace.WriteLine($"Notification email sent. Event: {eventName}, Email Address: {user.EmailAddress}, Success: {email.Successful}");

                if (!email.Successful)
                {
                    email.ErrorMessages.ForEach(message => Trace.TraceError($"Notification email failed to send. Event: {eventName}, Email Address: {user.EmailAddress}, Success: {email.Successful}"));
                }
            }
        }
    }
}

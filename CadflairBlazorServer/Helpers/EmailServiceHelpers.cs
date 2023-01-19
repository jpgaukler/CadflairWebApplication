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

            foreach (User user in await dataServicesManager.UserService.GetUsersBySubscriptionId(subscriptionId))
            {
                NotificationSetting setting = await dataServicesManager.NotificationService.GetNotificationSettingByNotificationIdAndUserId(notification.Id, user.Id);
                if (setting == null || !setting.IsEnabled) continue;
                addresses.Add(new Address(user.EmailAddress));
            }

            if (addresses.Count < 1) return;

            // pull out first user so the rest can be BCC'd
            string firstAddress = addresses[0].EmailAddress;
            addresses.RemoveAt(0);

            SendResponse email = await emailService.SetFrom("donotreply@cadflair.com")
                                                   .To(firstAddress)
                                                   .Subject(subject)
                                                   .BCC(addresses)
                                                   .UsingTemplateFromFile(Path.Combine(_templateRoot, templateFilename), model)
                                                   .SendAsync();

            Debug.WriteLine($"Notification email sent. Event name: {eventName} Success: {email.Successful}");

            if (!email.Successful)
            {
                foreach (string error in email.ErrorMessages)
                {
                    Debug.WriteLine($"Failed to send email: {error}");
                }
            }
        }
    }
}

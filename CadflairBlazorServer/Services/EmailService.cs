using FluentEmail.Core.Models;
using System.Reflection;

namespace CadflairBlazorServer.Services
{
    public class EmailService
    {
        private readonly DataServicesManager _dataServicesManager;
        private readonly IFluentEmail _emailService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(DataServicesManager dataServicesManager, IFluentEmail fluentEmail, ILogger<EmailService> logger)
        {
            _dataServicesManager = dataServicesManager;
            _emailService = fluentEmail;
            _logger = logger;
        }

        public async Task SendEmail(string toAddress, string subject, string bodyText, bool isHtml = false)
        {
            try
            {
                SendResponse email = await _emailService.SetFrom("donotreply@cadflair.com")
                                                        .To(toAddress)
                                                        .Subject(subject)
                                                        .Body(bodyText, isHtml)
                                                        .SendAsync();

                if (email.Successful)
                {
                    _logger.LogInformation($"Email sent successfully. Email Address: {toAddress}");
                }
                else
                {
                    email.ErrorMessages.ForEach(message => _logger.LogError($"Email failed to send.  Email Address: {toAddress} Error Message: {message}"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SendEmail)} failed: {ex}");
            }
        }

        public async Task SendEmailUsingTemplate<T>(string toAddress, string subject, string emailTemplatePath, T emailModel)
        {
            try
            {
                SendResponse email = await _emailService.SetFrom("donotreply@cadflair.com")
                                                        .To(toAddress)
                                                        .Subject(subject)
                                                        .UsingTemplateFromEmbedded(emailTemplatePath, emailModel, Assembly.GetExecutingAssembly())
                                                        .SendAsync();

                if (email.Successful)
                {
                    _logger.LogInformation($"Email sent successfully. Email Address: {toAddress}");
                }
                else
                {
                    email.ErrorMessages.ForEach(message => _logger.LogError($"Email failed to send.  Email Address: {toAddress} Error Message: {message}"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SendEmailUsingTemplate)} failed: {ex}");
            }
        }

        public async Task SendNotificationEmail<T>(int subscriptionId, int notificationId, string subject, string emailTemplatePath, T emailModel)
        {
            try
            {
                foreach (User user in await _dataServicesManager.UserService.GetUsersBySubscriptionId(subscriptionId))
                {
                    List<NotificationSetting> settings = await _dataServicesManager.NotificationService.GetNotificationSettingsByUserId(user.Id);
                    NotificationSetting? setting = settings.Find(i => i.Id == notificationId);

                    // create setting if it does not exist
                    if (setting == null)
                        setting = await _dataServicesManager.NotificationService.CreateNotificationSetting(notificationId, user.Id);

                    if (!setting.IsEnabled)
                        continue;

                    SendResponse email = await _emailService.SetFrom("donotreply@cadflair.com")
                                                            .To(user.EmailAddress)
                                                            .Subject(subject)
                                                            .UsingTemplateFromEmbedded(emailTemplatePath, emailModel, Assembly.GetExecutingAssembly())
                                                            .SendAsync();

                    if (email.Successful)
                    {
                        _logger.LogInformation($"Notification email sent. Notification Id: {notificationId}, Email Address: {user.EmailAddress}");
                    }
                    else
                    {
                        email.ErrorMessages.ForEach(message => _logger.LogError($"Notification email failed to send. Notification Id: {notificationId}, Email Address: {user.EmailAddress} Error Message: {message}"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SendNotificationEmail)} failed: {ex}");
            }
        }
    }
}

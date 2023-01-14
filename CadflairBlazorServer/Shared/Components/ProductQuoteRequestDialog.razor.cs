using CadflairDataAccess;
using CadflairDataAccess.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ProductQuoteRequestDialog
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] IFluentEmail _emailService { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;

        // parameters
        [CascadingParameter] public MudDialogInstance? MudDialog { get; set; }
        [Parameter] public Product Product{ get; set; } = default!;
        [Parameter] public ProductVersion ProductVersion { get; set; } = default!;
        [Parameter] public ProductConfiguration ProductConfiguration { get; set; } = default!;

        // fields
        private List<ILogicFormElement> _parameterGridItems = new();
        private bool _validInputs;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _emailAddress = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _phoneExtension = string.Empty;
        private string _messageText = string.Empty;

        private PatternMask _phoneMask = new("xxx-xxx-xxxx")
        {
            MaskChars = new[] { new MaskChar('x', @"[0-9]") }
        };

        protected override void OnInitialized()
        {
            // load parameters
            ILogicFormElement form = JsonConvert.DeserializeObject<ILogicFormElement>(ProductVersion.ILogicFormJson)!;
            form.SetParameterExpressions(ProductConfiguration.ArgumentJson);
            _parameterGridItems = form.GetParameterList();
        }

        private async Task Submit_OnClick()
        {
            if (!_validInputs) return;

            // create new record in db
            await _dataServicesManager.ProductService.CreateProductQuoteRequest(productConfigurationId: ProductConfiguration.Id,
                                                                                firstName: _firstName,
                                                                                lastName: _lastName,
                                                                                emailAddress: _emailAddress,
                                                                                phoneNumber: _phoneNumber,
                                                                                phoneExtension: _phoneExtension,
                                                                                messageText: _messageText);

            // send notifications to subscribing users
            Notification notification = await _dataServicesManager.NotificationService.GetNotificationByEventName("ProductQuoteRequest_Insert");

            foreach(User user in await _dataServicesManager.UserService.GetUsersBySubscriptionId(Product.SubscriptionId))
            {
                NotificationSetting setting = await _dataServicesManager.NotificationService.GetNotificationSettingByNotificationIdAndUserId(notification.Id, user.Id);
                if (setting.IsEnabled)
                {
                    _ = SendEmail(user.EmailAddress);
                }
            }

            _snackbar.Add("Request submitted!", Severity.Success);
            MudDialog?.Close(DialogResult.Ok(true));
        }

        private async Task SendEmail(string emailAddress)
        {
            StringBuilder template = new();
            //template.AppendLine("Dear @Model.FirstName,");
            //template.AppendLine("<p>Thanks for purchasing @Model.ProductName. We hope you enjoy it.</p>");
            //template.AppendLine("- Cadflair");

            template.AppendLine("<h1>You've got a new request!</h1>");
            template.AppendLine("<p>Click here to view > </p>");
            template.AppendLine("<a href='http://www.cadflair.com/'>Cadflair link</a>");
            template.AppendLine("- Cadflair");


            //foreach(string i in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
            //{
            //    Debug.WriteLine(i);
            //}

            SendResponse email = await _emailService.SetFrom("donotreply@cadflair.com")
                                                    .To(emailAddress)
                                                    .Subject("New Request!")
                                                    //.Body("Test message")
                                                    //.UsingTemplate(template.ToString(), new { FirstName = "Justin", ProductName = "Cadflair Pro" })
                                                    .UsingTemplate(template.ToString(), new { })
                                                    //.UsingTemplateFromEmbedded("CadflairBlazorServer.EmailTemplates.ThankYouTemplate.cshtml", new { }, ThisAssembly())
                                                    .SendAsync();

            Debug.WriteLine($"MessageId: {email.MessageId}, Successful: {email.Successful}");

            foreach (string error in email.ErrorMessages)
            {
                Debug.WriteLine($"Error: {error}");
            }

        }
    }
}
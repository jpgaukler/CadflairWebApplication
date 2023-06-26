using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ContactUsForm
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] EmailService _emailService { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] ILogger<ContactUsForm> _logger { get; set; } = default!;

        // fields
        private Variant _variant = Variant.Outlined;
        private string? _firstName;
        private string? _lastName;
        private string? _companyName;
        private string? _emailAddress;
        private string? _message;
        private bool _isValid;

        private async Task Submit_OnClick()
        {
            if (!_isValid)
                return;

            try
            {
                string bodyText = $"""
                                  I want to know more about Cadflair. Please add me to your mailing list!

                                  Name: {_firstName} {_lastName}
                                  Email: {_emailAddress} 
                                  Company: {_companyName} 

                                  Message: 
                                  {_message} 
                                  """;

                await _dataServicesManager.NotificationService.CreateContactRequest(_firstName, _lastName, _emailAddress, _companyName, _message);

                await _emailService.SendEmail(toAddress: "justin.gaukler@verizon.net", subject: "Keep Me Informed!", bodyText: bodyText);
                _snackbar.Add("Message submitted!", Severity.Success);

                // clear form
                _firstName = null;
                _lastName = null;
                _emailAddress = null;
                _companyName = null;
                _message = null;
            }
            catch (Exception ex)
            {
                _snackbar.Add("An unknown error occurred!", Severity.Error);
                _logger.LogError(ex, $"An unknown error occurred!");
            }
        }
    }
}
using CadflairDataAccess;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ProductQuoteRequestDialog
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;

        // parameters
        [CascadingParameter] public MudDialogInstance? MudDialog { get; set; }
        [Parameter] public int ProductConfigurationId { get; set; }

        // fields
        private bool _validInputs;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _emailAddress = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _phoneExtension = string.Empty;
        private string _messageText = string.Empty;

        private async Task Submit_OnClick()
        {
            if (!_validInputs) return;
            await _dataServicesManager.ProductService.CreateProductQuoteRequest(productConfigurationId: ProductConfigurationId, 
                                                                                firstName: _firstName, 
                                                                                lastName: _lastName, 
                                                                                emailAddress: _emailAddress, 
                                                                                phoneNumber: _phoneNumber, 
                                                                                phoneExtension: _phoneExtension, 
                                                                                messageText: _messageText);

            _snackbar.Add("Request submitted!", Severity.Success);
            MudDialog?.Close(DialogResult.Ok(true));
        }

        private void Cancel_OnClick() => MudDialog?.Cancel();
    }
}
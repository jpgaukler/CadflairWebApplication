using CadflairDataAccess;
using CadflairDataAccess.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ProductQuoteRequestDialog
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
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
            await _dataServicesManager.ProductService.CreateProductQuoteRequest(productConfigurationId: ProductConfiguration.Id,
                                                                                firstName: _firstName,
                                                                                lastName: _lastName,
                                                                                emailAddress: _emailAddress,
                                                                                phoneNumber: _phoneNumber,
                                                                                phoneExtension: _phoneExtension,
                                                                                messageText: _messageText);

            _snackbar.Add("Request submitted!", Severity.Success);
            MudDialog?.Close(DialogResult.Ok(true));
        }
    }
}
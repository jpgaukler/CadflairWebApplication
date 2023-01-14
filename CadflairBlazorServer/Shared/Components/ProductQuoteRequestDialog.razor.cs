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

        private void Submit_OnClick()
        {
            if (!_validInputs) return;

            ProductQuoteRequest request = new()
            {
                ProductConfigurationId = ProductConfiguration.Id,
                FirstName = _firstName,
                LastName = _lastName,
                EmailAddress = _emailAddress,
                PhoneNumber = _phoneNumber,
                PhoneExtension = _phoneExtension,
                MessageText = _messageText
            };

            MudDialog?.Close(DialogResult.Ok(request));
        }
    }
}
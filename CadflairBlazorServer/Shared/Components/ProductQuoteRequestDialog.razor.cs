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
        [Parameter] public string? FirstName { get; set; } 
        [Parameter] public string? LastName { get; set; } 
        [Parameter] public string? EmailAddress { get; set; } 
        [Parameter] public string? PhoneNumber  { get; set; } 
        [Parameter] public string? PhoneExtension  { get; set; } 
        [Parameter] public string? MessageText { get; set; } 

        // fields
        private List<ILogicFormElement> _parameterGridItems = new();
        private bool _validInputs;

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

            MudDialog?.Close(DialogResult.Ok(this));
        }
    }
}
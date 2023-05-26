using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Reflection;

namespace CadflairBlazorServer.Pages
{
    public partial class Index
    {
        // services
        [Inject] AuthenticationService _authenticationService { get; set; } = default!;
        [Inject] IFluentEmail _emailService { get; set; } = default!;
        [Inject] IDialogService  _dialogService { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] IJSRuntime _js { get; set; } = default!;

        //protected override async Task OnInitializedAsync()
        //{
        //    //if (await _authenticationService.IsLoggedInUserValid() == false)
        //    //{
        //    //    return;
        //    //}
        //}

        private async Task EmailTest_OnClick()
        {

            ProductQuoteRequestEmailModel model = new()
            {
                CustomerName = "Customer",
                ProductName = "Product"
            };

            SendResponse email = await _emailService.SetFrom("donotreply@cadflair.com")
                                                    .To("justin.gaukler@verizon.net")
                                                    .Subject("TEST EMAIL")
                                                    .UsingTemplateFromEmbedded(model.Path, model, Assembly.GetExecutingAssembly())
                                                    .SendAsync();

            Trace.WriteLine($"Success? {email.Successful}");

            if (!email.Successful)
                email.ErrorMessages.ForEach(message => Trace.TraceError($"{message}"));

        }

    }
}
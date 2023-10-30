using CadflairBlazorServer.Pages.McMaster_Idea;
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages
{
    public partial class Index
    {
        // services
        [Inject] DataServicesManager  DataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        //protected override async Task OnInitializedAsync()
        //{
        //    //if (await _authenticationService.IsLoggedInUserValid() == false)
        //    //{
        //    //    return;
        //    //}
        //}

        //private async Task EmailTest_OnClick()
        //{
        //    if (await _authenticationService.IsLoggedInUserValid() == false)
        //        return;

        //    // send welcome email
        //    WelcomeEmailModel model = new()
        //    {
        //        Name = _authenticationService.LoggedInUser!.FullName
        //    };

        //    _ = _emailService.SendEmail(toAddress: _authenticationService.LoggedInUser.EmailAddress,
        //                                subject: "Welcome to Cadflair!",
        //                                emailTemplatePath: model.Path,
        //                                emailModel: model);


        //    ProductQuoteRequestEmailModel model2 = new()
        //    {
        //        CustomerName = "Customer",
        //        ProductName = "Product"
        //    };

        //    _ = _emailService.SendNotificationEmail(subscriptionId: (int)_authenticationService.LoggedInUser.SubscriptionId!,
        //                                            notificationId: (int)NotificationIdEnum.ProductQuoteRequest_Insert,
        //                                            subject: "New Request!",
        //                                            emailTemplatePath: model2.Path,
        //                                            emailModel: model2);


        //}

        private async Task LearnMore_OnClick()
        {
            await JSRuntime.InvokeVoidAsync("anchorLink.scrollIntoView", "learn-more-tag");
        }

    }
}
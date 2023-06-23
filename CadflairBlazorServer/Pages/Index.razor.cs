using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Reflection;

namespace CadflairBlazorServer.Pages
{
    public partial class Index
    {
        // services
        [Inject] AuthenticationService _authenticationService { get; set; } = default!;
        [Inject] EmailService _emailService { get; set; } = default!;
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

    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CadflairBlazorServer.Pages
{
    public partial class NewSubscription
    {
        // services
        [Inject] NavigationManager _navigationManager { get; set; } = default!;
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] EmailService _emailService { get; set; } = default!;

        // fields
        private User _loggedInUser = new();
        private SubscriptionType? _subscriptionType;
        private List<SubscriptionType> _subscriptionTypes = new();
        private bool _validInputs;
        private string _companyName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _loggedInUser = await _authenticationStateProvider.GetUser(_dataServicesManager);
            _subscriptionTypes = await _dataServicesManager.SubscriptionService.GetSubscriptionTypes();

            if (_loggedInUser.SubscriptionId != null)
            {
                _navigationManager.NavigateTo("/dashboard");
                return;
            }
        }

        private async Task Submit_OnClick()
        {
            if (!_validInputs) 
                return;

            // create new subscription record
            Subscription newSubscription = await _dataServicesManager.SubscriptionService.CreateSubscription(subscriptionTypeId: _subscriptionType!.Id,
                                                                                                             companyName: _companyName, 
                                                                                                             ownerId: _loggedInUser.Id, 
                                                                                                             createdById: _loggedInUser.Id);

            // update the user
            _loggedInUser.SubscriptionId = newSubscription.Id;
            await _dataServicesManager.UserService.UpdateUser(_loggedInUser);

            // create 'Products' folder to act as the root directory
            await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId: newSubscription.Id, 
                                                                          createdById: _loggedInUser.Id, 
                                                                          displayName: "Products", 
                                                                          parentId: null);


            // send welcome email
            WelcomeEmailModel model = new()
            {
                Name = _loggedInUser.FullName
            };

            _ = _emailService.SendEmail(toAddress: _loggedInUser.EmailAddress,
                                        subject: "Welcome to Cadflair!",
                                        emailTemplatePath: model.Path,
                                        emailModel: model);

            _navigationManager.NavigateTo("/dashboard");
        }
    }
}
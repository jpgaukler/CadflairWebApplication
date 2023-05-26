using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages
{
    public partial class NewSubscription
    {
        // services
        [Inject] NavigationManager _navigationManager { get; set; } = default!;
        [Inject] AuthenticationService _authenticationService { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] EmailService _emailService { get; set; } = default!;

        // fields
        private SubscriptionType? _subscriptionType;
        private List<SubscriptionType> _subscriptionTypes = new();
        private bool _validInputs;
        private string _companyName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (await _authenticationService.IsLoggedInUserValid() == false)
            {
                _navigationManager.NavigateTo("/");
                return;
            }

            if (_authenticationService.LoggedInUser?.SubscriptionId != null)
            {
                _navigationManager.NavigateTo("/dashboard");
                return;
            }

            _subscriptionTypes = await _dataServicesManager.SubscriptionService.GetSubscriptionTypes();
        }

        private async Task Submit_OnClick()
        {
            if (_authenticationService.LoggedInUser == null) 
                return;

            if (!_validInputs) 
                return;

            // create new subscription record
            Subscription newSubscription = await _dataServicesManager.SubscriptionService.CreateSubscription(subscriptionTypeId: _subscriptionType!.Id,
                                                                                                             companyName: _companyName, 
                                                                                                             ownerId: _authenticationService.LoggedInUser.Id, 
                                                                                                             createdById: _authenticationService.LoggedInUser.Id);

            // update the user
            _authenticationService.LoggedInUser.SubscriptionId = newSubscription.Id;
            await _dataServicesManager.UserService.UpdateUser(_authenticationService.LoggedInUser);

            // create 'Products' folder to act as the root directory
            await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId: newSubscription.Id, 
                                                                          createdById: _authenticationService.LoggedInUser.Id, 
                                                                          displayName: "Products", 
                                                                          parentId: null);


            // send welcome email
            WelcomeEmailModel model = new()
            {
                Name = _authenticationService.LoggedInUser.FullName
            };

            _ = _emailService.SendEmail(toAddress: _authenticationService.LoggedInUser.EmailAddress,
                                        subject: "Welcome to Cadflair!",
                                        emailTemplatePath: model.Path,
                                        emailModel: model);

            _navigationManager.NavigateTo("/dashboard");
        }
    }
}
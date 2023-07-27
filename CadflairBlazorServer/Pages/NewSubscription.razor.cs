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
        private User? _loggedInUser;
        private SubscriptionType? _subscriptionType;
        private List<SubscriptionType> _subscriptionTypes = new();
        private bool _validInputs;
        private string _companyName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _loggedInUser = await _authenticationService.GetUser();

            if (_loggedInUser == null)
            {
                _navigationManager.NavigateTo("/");
                return;
            }

            if (_loggedInUser.SubscriptionId != null)
            {
                _navigationManager.NavigateTo("/dashboard");
                return;
            }

            _subscriptionTypes = await _dataServicesManager.SubscriptionService.GetSubscriptionTypes();
        }

        private async Task Submit_OnClick()
        {
            if (_loggedInUser == null) 
                return;

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
            await _dataServicesManager.CatalogService.CreateCatalogFolder(subscriptionId: newSubscription.Id, 
                                                                          createdById: _loggedInUser.Id, 
                                                                          displayName: "Products", 
                                                                          parentId: null);


            // send welcome email
            WelcomeEmailModel model = new()
            {
                Name = _loggedInUser.FullName
            };

            _ = _emailService.SendEmailUsingTemplate(toAddress: _loggedInUser.EmailAddress,
                                                     subject: "Welcome to Cadflair!",
                                                     emailTemplatePath: model.Path,
                                                     emailModel: model);

            _navigationManager.NavigateTo("/dashboard");
        }
    }
}
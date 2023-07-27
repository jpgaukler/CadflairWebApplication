using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages
{
    public partial class Dashboard
    {
        // services
        [Inject] AuthenticationService _authenticationService { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager _forgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private List<ProductQuoteRequest> _productQuoteRequests = new();
        private User? _loggedInUser;
        private Subscription _subscription = new();
        private bool _showGetStarted = false;

        protected override async Task OnInitializedAsync()
        {

            _loggedInUser = await _authenticationService.GetUser();

            if (_loggedInUser == null)
            {
                _navigationManager.NavigateTo("/notauthorized");
                return;
            }

            if (_loggedInUser.SubscriptionId == null)
            {
                _showGetStarted = true;
                return;
            }

            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionById((int)_loggedInUser.SubscriptionId!);
            _productQuoteRequests = await _dataServicesManager.ProductService.GetProductQuoteRequestsBySubscriptionId(_subscription.Id);
        }

        private void UpdateUI(object? sender, EventArgs e)
        {
            // TO DO: update the UI if impersonation has been set
        }

        private void ProductQuoteRequestGrid_RowClick(DataGridRowClickEventArgs<ProductQuoteRequest> args)
        {
            Debug.WriteLine($"{args.Item.FirstName} {args.Item.LastName}");
        }

    }
}
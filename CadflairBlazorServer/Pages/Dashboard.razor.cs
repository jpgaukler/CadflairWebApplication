using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages
{
    public partial class Dashboard : IDisposable
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
        private Subscription _subscription = new();
        private bool _showGetStarted = false;

        protected override async Task OnInitializedAsync()
        {
            _authenticationService.OnImpersonateUserSet += UpdateUI;

            if (await _authenticationService.IsLoggedInUserValid() == false)
            {
                _navigationManager.NavigateTo("/notauthorized");
                return;
            }

            if (_authenticationService.LoggedInUser?.SubscriptionId == null)
            {
                _showGetStarted = true;
                return;
            }

            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionById((int)_authenticationService.LoggedInUser?.SubscriptionId!);
            _productQuoteRequests = await _dataServicesManager.ProductService.GetProductQuoteRequestsBySubscriptionId(_subscription.Id);
        }

        private void UpdateUI(object? sender, EventArgs e)
        {
            // TO DO: update the UI if impersonation has been set
        }

        private void ProductQuoteRequestGrid_RowClick(DataGridRowClickEventArgs<ProductQuoteRequest> args)
        {

        }

        public void Dispose() => _authenticationService.OnImpersonateUserSet -= UpdateUI;
    }
}
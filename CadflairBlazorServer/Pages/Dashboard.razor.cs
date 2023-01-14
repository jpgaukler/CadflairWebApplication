using Newtonsoft.Json;
using CadflairDataAccess.Models;
using CadflairBlazorServer.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using CadflairDataAccess;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;

namespace CadflairBlazorServer.Pages
{
    public partial class Dashboard
    {
        // services
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager _forgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private User _loggedInUser = new();
        private Subscription _subscription = new();
        private bool _showGetStarted = false;

        protected override async Task OnInitializedAsync()
        {
            _loggedInUser = await _authenticationStateProvider.GetUser(_dataServicesManager);

            if (_loggedInUser.SubscriptionId == null)
            {
                _showGetStarted = true;
                return;
            }

            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
        }
    }
}
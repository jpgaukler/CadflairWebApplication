using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models.Security;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class McMasterCatalogEditor
{
    // services
    [Inject] BlobServiceClient BlobServiceClient { get; set; } = default!;
    [Inject] AuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] ILogger<McMasterCatalogEditor> Logger { get; set; } = default!;

    // parameters
    [Parameter] public string CompanyName { get; set; } = string.Empty;

    // fields
    private bool _drawerOpen = true;
    private bool _initializing = true;

    private User? _loggedInUser;
    private Subscription? _subscription;
    private List<Category> _categories = new();
    private List<ProductDefinition> _productDefinitions = new();


    protected override async Task OnInitializedAsync()
    {
        try
        {
            // get data
            _loggedInUser = await AuthenticationService.GetUser();

            if (_loggedInUser == null || _loggedInUser.SubscriptionId == null)
            {
                NavigationManager.NavigateTo("/notauthorized");
                return;
            }

            _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionById((int)_loggedInUser.SubscriptionId!);
            _categories = await DataServicesManager.McMasterService.GetCategoriesBySubscriptionId(_subscription.Id);
            _productDefinitions = await DataServicesManager.McMasterService.GetProductDefinitionsBySubscriptionId(_subscription.Id);
            _initializing = false;

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
            NavigationManager.NavigateTo("/notfound");
            return;
        }
    }

}
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class CatalogEditor
{
    // services
    [Inject] BlobServiceClient BlobServiceClient { get; set; } = default!;
    [Inject] AuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] ILogger<CatalogEditor> Logger { get; set; } = default!;

    // parameters
    [Parameter] public string CompanyName { get; set; } = string.Empty;

    // fields
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
            _categories = await DataServicesManager.CatalogService.GetCategoriesBySubscriptionId(_subscription.Id);
            _productDefinitions = await DataServicesManager.CatalogService.GetProductDefinitionsBySubscriptionId(_subscription.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
            NavigationManager.NavigateTo("/notfound");
            return;
        }
    }

}
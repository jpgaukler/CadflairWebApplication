using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class Categories
    {
        // services
        [Inject] DataServicesManager  DataServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ILogger<Categories> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private Subscription? _subscription;
        private List<Category> _categories = new();
        private Category? _selectedCategory;
        private List<ProductDefinition> _productDefinitions = new();
        private bool _drawerOpen = true;
        private bool _initializing = true;

        protected override async Task OnInitializedAsync()
        {
            // get data
            try
            {
                _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
                _categories = await DataServicesManager.McMasterService.GetCategoriesBySubscriptionId(_subscription.Id);
                _productDefinitions = await DataServicesManager.McMasterService.GetProductDefinitionsBySubscriptionId(_subscription.Id);
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing McMasterCatalog page!");
                NavigationManager.NavigateTo("/notfound");
                return;
            }
        }

        private void Category_OnClick(Category? selectedCategory)
        {
            _selectedCategory = selectedCategory;
        }
    }
}
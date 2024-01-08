using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class Categories
{
    // services
    [Inject] DataServicesManager  DataServicesManager { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] ILogger<Categories> Logger { get; set; } = default!;

    // parameters
    [Parameter] public string CompanyName { get; set; } = string.Empty;
    [Parameter] public string? CategoryName { get; set; } 

    // fields
    private Subscription? _subscription;
    private List<Category> _categories = new();
    private Category? _category;
    private List<BreadcrumbItem> _breadcrumbItems = new();
    private List<ProductDefinition> _productDefinitions = new();
    private bool _initializing = true;

    protected override async Task OnInitializedAsync()
    {
        // get data
        try
        {
            _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
            _categories = await DataServicesManager.CatalogService.GetCategoriesBySubscriptionId(_subscription.Id);
            _productDefinitions = await DataServicesManager.CatalogService.GetProductDefinitionsBySubscriptionId(_subscription.Id);
            _initializing = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while initializing McMasterCatalog page!");
            NavigationManager.NavigateTo("/notfound");
            return;
        }
    }

    protected override void OnParametersSet()
    {
        _category = _categories.ToFlatList().FirstOrDefault(i => i.Name == CategoryName);

        // set breadcrumbs
        _breadcrumbItems.Clear();

        if(_category != null)
        {
            // add selected folder 
            Category category = _category;
            _breadcrumbItems.Add(new BreadcrumbItem(text: category.Name, href: $"{CompanyName}/categories/{category.Name}/"));

            // add parents
            while (category.ParentCategory != null)
            {
                category = category.ParentCategory;
                _breadcrumbItems.Add(new BreadcrumbItem(text: category.Name, href: $"{CompanyName}/categories/{category.Name}/"));
            }
        }

        // reverse the list so the breadcrumbs are displayed from the top down
        _breadcrumbItems.Add(new BreadcrumbItem(text: "All Categories", href: $"{CompanyName}/categories/"));
        _breadcrumbItems.Reverse();
    }

    private void Category_OnClick(Category selectedCategory)
    {
        //_selectedCategory = selectedCategory;
        NavigationManager.NavigateTo($"{CompanyName}/categories/{selectedCategory.Name}/");
    }

    private void ProductDefinition_OnClick(ProductDefinition productDefinition)
    {
        NavigationManager.NavigateTo($"{CompanyName}/products/{productDefinition.Name}/");
    }
}
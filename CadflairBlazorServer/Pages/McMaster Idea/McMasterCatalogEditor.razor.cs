using CadflairBlazorServer.Pages.McMaster_Idea.Models;
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class McMasterCatalogEditor
{
    // services
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] ILogger<McMasterCatalogEditor> Logger { get; set; } = default!;

    // parameters

    // fields
    private bool _drawerOpen = true;
    private bool _initializing = true;

    private List<Category> _categories = new();
    private List<ProductDefinition> _productDefinitions = new();


    protected override async Task OnInitializedAsync()
    {
        try
        {
            _categories = DummyData.GetCategories();
            _productDefinitions = DummyData.GetProductDefinitions();
            await Task.Delay(1000);
            _initializing = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }

}
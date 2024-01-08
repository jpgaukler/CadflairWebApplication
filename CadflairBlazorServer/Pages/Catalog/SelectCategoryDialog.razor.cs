using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class SelectCategoryDialog
{
    // services
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;

    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
    [Parameter] public Category? SelectedCategory { get; set; }
    [Parameter] public int SubscriptionId { get; set; }

    //fields
    private List<Category> _categories = new();

    protected override async Task OnInitializedAsync()
    {
        var options = new DialogOptions 
        { 
            CloseButton = false,
            DisableBackdropClick = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseOnEscapeKey = false 
        };


        MudDialogInstance.SetOptions(options);
        _categories = await DataServicesManager.CatalogService.GetCategoriesBySubscriptionId(SubscriptionId);
    }

    private void Cancel_OnClick() => MudDialogInstance.Cancel();
    private void Ok_OnClick() => MudDialogInstance.Close(this);
}
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ProductDefinitionGridItem
{

    // parameters
    [Parameter] public ProductDefinition ProductDefinition { get; set; } = new();
    [Parameter] public EventCallback<ProductDefinition> ProductDefinitionClicked { get; set; }


    private async Task ProductDefinition_OnClick()
    {
        await ProductDefinitionClicked.InvokeAsync(ProductDefinition);
    }

}
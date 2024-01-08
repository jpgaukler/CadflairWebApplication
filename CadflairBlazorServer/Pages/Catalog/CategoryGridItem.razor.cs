using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class CategoryGridItem
{

    // parameters
    [Parameter] public Category Category { get; set; } = new();
    [Parameter] public EventCallback<Category> CategoryClicked { get; set; }


    private async Task Category_OnClick()
    {
        await CategoryClicked.InvokeAsync(Category);
    }

}
using CadflairBlazorServer.Pages.McMaster_Idea.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ManageCategories
{
    // services

    // parameters
    [Parameter] public List<Category> Categories { get; set; } = new();

    // fields
    private Category? _selectedCategory;
    private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
    private string _dragStyle = _initialDragStyle;

    private void AddCategory_OnClick(Category? parentCategory)
    {
        Category newCategory = new()
        {
            Name = "New Category",
        };

        if (parentCategory == null)
            Categories.Add(newCategory);
        else
            parentCategory.ChildCategories.Add(newCategory);
    }

    private void DeleteCategory_OnClick(Category? parentCategory)
    {
        // TO DO: delete product category and update linked entities (other categories and product definitions)
    }

    private void UploadThumbnail(IBrowserFile file)
    {
        //TODO upload the files to the server
    }
    private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
    private void ClearDragStyle() => _dragStyle = _initialDragStyle;

}
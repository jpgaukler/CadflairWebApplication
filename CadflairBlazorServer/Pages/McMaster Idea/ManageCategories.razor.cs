using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ManageCategories
{
    // services
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    // parameters
    [CascadingParameter] public User LoggedInUser { get; set; } = default!;
    [CascadingParameter] public Subscription Subscription { get; set; } = default!;
    [Parameter] public List<Category> Categories { get; set; } = default!;

    // fields
    private Category? _selectedCategory;
    private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
    private string _dragStyle = _initialDragStyle;


    private async Task AddCategory_OnClick(Category? parentCategory)
    {

        DialogResult result = await DialogService.Show<AddCategoryDialog>("Add Category").Result;

        if (result.Canceled)
            return;

        AddCategoryDialog dialog = (AddCategoryDialog)result.Data;

        // check for duplicate category name
        if (!IsCategoryNameUnique(dialog.Name!))
        {
            Snackbar.Add("Category name already used!", Severity.Warning);
            return;
        }

        Category newCategory = await DataServicesManager.McMasterService.CreateCategory(subscriptionId: Subscription.Id,
                                                                                        parentId: _selectedCategory?.Id,
                                                                                        name: dialog.Name,
                                                                                        description: dialog.Description,
                                                                                        thumbnailId: null,
                                                                                        createdById: LoggedInUser.Id);


        if (_selectedCategory == null)
        {
            Categories.Add(newCategory);
            Categories.Sort();
        }
        else
        {
            newCategory.ParentCategory = _selectedCategory;
            _selectedCategory.ChildCategories.Add(newCategory);
            _selectedCategory.ChildCategories.Sort();
        }
    }

    private bool IsCategoryNameUnique(string name)
    {
        if (_selectedCategory == null)
            return !Categories.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return !_selectedCategory.ChildCategories.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
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
    private string _nameField = string.Empty;
    private string? _descriptionField; 
    private bool _isDirty;

    private void Category_OnClick(Category? category)
    {
        _selectedCategory = category;

        if (_selectedCategory == null)
            return;

        _nameField = _selectedCategory.Name;
        _descriptionField = _selectedCategory?.Description;
    }

    private void Name_ValueChanged(string value)
    {
        _isDirty = true;
        _nameField = value;
    }

    private void Description_ValueChanged(string? value)
    {
        _isDirty = true;
        _descriptionField = value;
    }

    private async Task UpdateCategory_OnClick()
    {
        if (_selectedCategory == null)
            return;

        if (_isDirty == false)
            return;

        if (string.IsNullOrWhiteSpace(_nameField))
            return;

        _selectedCategory.Name = _nameField;
        _selectedCategory.Description = _descriptionField;
        await DataServicesManager.McMasterService.UpdateCategory(_selectedCategory);
        _isDirty = false;

        Snackbar.Add("Changes saved!", Severity.Success);
    }

    private async Task AddCategory_OnClick(Category? parentCategory)
    {

        DialogResult result = await DialogService.Show<AddCategoryDialog>("Add Category").Result;

        if (result.Canceled)
            return;

        AddCategoryDialog dialog = (AddCategoryDialog)result.Data;

        // check for duplicate category name
        if (Categories.ToFlatList().Any(i => i.Name.Equals(dialog.Name, StringComparison.OrdinalIgnoreCase)))
        {
            Snackbar.Add("Category name already used!", Severity.Warning);
            return;
        }

        Category newCategory = await DataServicesManager.McMasterService.CreateCategory(subscriptionId: Subscription.Id,
                                                                                        parentId: _selectedCategory?.Id,
                                                                                        name: dialog.Name,
                                                                                        description: dialog.Description,
                                                                                        thumbnailUri: null,
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

    private async Task DeleteCategory_OnClick()
    {
        // TO DO: delete product category and update linked entities (other categories and product definitions)
        if (_selectedCategory == null)
            return;

        if (_selectedCategory.ChildCategories.Any())
        {
            Snackbar.Add("Selected category contains sub-categories!", Severity.Warning);
            return;
        }

        bool? confirmDelete = await DialogService.ShowMessageBox(title: "Delete Category",
                                                                 message: "Are you sure you want to delete this Category?",
                                                                 yesText: "Yes",
                                                                 cancelText: "Cancel");
        if (confirmDelete != true)
            return;

        // TO DO: need to delete the thumbnail from blob storage if there is one

        await DataServicesManager.McMasterService.DeleteCategoryById(_selectedCategory.Id);

        if(_selectedCategory.ParentCategory == null)
        {
            Categories.Remove(_selectedCategory);
            _selectedCategory = null;
        }
        else
        {
            _selectedCategory.ParentCategory.ChildCategories.Remove(_selectedCategory);
            Category_OnClick(_selectedCategory.ParentCategory);
        }
    }

    private async Task UpdateThumbnail(string? thumbnailUri)
    {
        if(_selectedCategory == null)
            return;

        _selectedCategory.ThumbnailUri = thumbnailUri;
        await DataServicesManager.McMasterService.UpdateCategory(_selectedCategory);

        Snackbar.Add($"Thumbnail updated!", Severity.Success);
    }

}
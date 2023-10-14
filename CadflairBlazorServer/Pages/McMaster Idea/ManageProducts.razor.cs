using CadflairBlazorServer.Pages.McMaster_Idea.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ManageProducts
{
    // services
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    // parameters
    [Parameter] public List<ProductDefinition> ProductDefinitions { get; set; } = new();

    // fields
    private ProductDefinition? _selectedProductDefinition;
    private ProductTable _productTable = new();
    private List<Product> _products = new();
    private Product _newProduct= new();
    private List<string> _events = new();


    private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
    private string _dragStyle = _initialDragStyle;

    private void ProductDefinition_OnClick(ProductDefinition productDefinition)
    {
        _selectedProductDefinition = productDefinition;
        _productTable = DummyData.GetProductTableByProductDefinitionId(_selectedProductDefinition.Id);
        ResetNewProduct_OnClick();

        // TO DO: load the product table
    }

    private void AddProductDefinition_OnClick()
    {
        ProductDefinitions.Add(new()
        {
            Name = "New product definition"
        });
    }

    private void AddColumnDefinition_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        _productTable.ColumnDefinitions.Add(new()
        {
            Id = _productTable.ColumnDefinitions.Count + 1,
            Header = "New Column",
        });

        ResetNewProduct_OnClick();

        AddEvent($"Event = NewColumnDefinition");
    }

    private void ResetNewProduct_OnClick()
    {
        if (_selectedProductDefinition == null) 
            return;
        
        _newProduct = new()
        {
            ColumnValues = _selectedProductDefinition.ProductTable.ColumnDefinitions.Select(i => new ColumnValue()
            {
                ColumnDefinitionId = i.Id,
            }).ToList()
        };

        //StateHasChanged();
    }

    private void AddProduct_OnClick()
    {
        _products.Add(_newProduct);
        ResetNewProduct_OnClick();

        AddEvent($"Event = NewProductRecord");
    }

    private void BackupItem(Product product)
    {
        // TO DO: create a backup copy of the item in memory
        //elementBeforeEdit = new()
        //{
        //    Sign = ((Element)element).Sign,
        //    Name = ((Element)element).Name,
        //    Molar = ((Element)element).Molar,
        //    Position = ((Element)element).Position
        //};

        AddEvent($"RowEditPreview event: made a backup of record {product.Id}");
        StateHasChanged();
    }

    private void UpdateProduct_OnClick(Product product)
    {
        // TO DO: save changes to database

        AddEvent($"RowEditCommit event: Changes to record: {product.Id} values: {string.Join(", ", product.ColumnValues.Select(i => i.Value))} committed");
    }

    private void ResetItemToOriginalValues(Product product)
    {
        // TO DO: reset to original values

        
        AddEvent($"RowEditCancel event: Editing of record: {product.Id} values: {string.Join(", ", product.ColumnValues.Select(i => i.Value))} canceled");
        StateHasChanged();
    }

    private void AddEvent(string message)
    {
        _events.Insert(0, message);
        StateHasChanged();
    }

    private void UploadThumbnail(IBrowserFile file)
    {
        //TODO upload the files to the server
    }

    private async Task Attachments_FilesChanged(IReadOnlyList<IBrowserFile> files)
    {

    }

    private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
    private void ClearDragStyle() => _dragStyle = _initialDragStyle;

}
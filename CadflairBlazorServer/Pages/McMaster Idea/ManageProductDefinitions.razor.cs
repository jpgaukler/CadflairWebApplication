using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ManageProductDefinitions
{
    // services
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;

    // parameters
    [CascadingParameter] public User LoggedInUser { get; set; } = default!;
    [CascadingParameter] public Subscription Subscription { get; set; } = default!;
    [Parameter] public List<ProductDefinition> ProductDefinitions { get; set; } = new();

    // fields
    private ProductDefinition? _selectedProductDefinition;
    private ProductTable? _productTable;
    private Row _rowBeforeEdit = new();
    private List<string> _events = new();


    private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
    private string _dragStyle = _initialDragStyle;

    private async Task ProductDefinition_OnClick(ProductDefinition productDefinition)
    {
        _selectedProductDefinition = productDefinition;
        _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(productDefinition.Id);
        //ResetNewRow_OnClick();
    }

    private async Task DeleteProductDefinition_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        await DataServicesManager.McMasterService.DeleteProductDefinitionById(_selectedProductDefinition.Id);
        ProductDefinitions.Remove(_selectedProductDefinition);
        _selectedProductDefinition = null;
    }

    private async Task AddProductDefinition_OnClick()
    {
        DialogResult result = await DialogService.Show<AddProductDefinitionDialog>("Add Product Definition").Result;

        if (result.Canceled)
            return;

        AddProductDefinitionDialog dialog = (AddProductDefinitionDialog)result.Data;

        ProductDefinition newProductDefinition = await DataServicesManager.McMasterService.CreateProductDefinition(subscriptionId: Subscription.Id,
                                                                                                                   categoryId: null,
                                                                                                                   name: dialog.Name,
                                                                                                                   description: dialog.Description,
                                                                                                                   thumbnailId: null,
                                                                                                                   forgeBucketKey: null,
                                                                                                                   createdById: LoggedInUser.Id);

        ProductDefinitions.Add(newProductDefinition);
    }

    private async Task EditColumns_OnClick()
    {
        if (_productTable == null)
            return;

        DialogParameters parameters = new()
        {
            { nameof(EditColumnsDialog.ProductTable), _productTable},
            { nameof(EditColumnsDialog.LoggedInUser), LoggedInUser},
        };

        await DialogService.Show<EditColumnsDialog>("Edit Columns", parameters).Result;
    }

    private async Task AddRow_OnClick()
    {
        if (_productTable == null) 
            return;

        DialogParameters parameters = new()
        {
            { nameof(AddRowDialog.Columns), _productTable.Columns },
        };

        DialogResult result = await DialogService.Show<AddRowDialog>("Add Product", parameters).Result;

        if (result.Canceled)
            return;

        AddRowDialog dialog = (AddRowDialog)result.Data;

        Row newRow = await DataServicesManager.McMasterService.CreateRow(productTableId: _productTable.Id,
                                                                         createdById: LoggedInUser.Id);

        // add an new table value for each column 
        foreach(var tableValue in dialog.NewRowValues)
        {
            TableValue newTableValue = await DataServicesManager.McMasterService.CreateTableValue(productTableId: _productTable.Id,
                                                                                                  rowId: newRow.Id,
                                                                                                  columnId: tableValue.ColumnId,
                                                                                                  value: tableValue.Value,
                                                                                                  createdById: LoggedInUser.Id);
            newRow.TableValues.Add(newTableValue);
        }

        _productTable.Rows.Add(newRow);

        //ResetNewRow_OnClick();
        //AddEvent($"Event = NewProductRecord");
    }

    private void RowEditPreview(Row row)
    {
        // create a backup copy of the values in memory
        _rowBeforeEdit = new()
        {
            TableValues = row.TableValues.Select(i => new TableValue()
            {
                Id = i.Id,
                RowId = i.RowId,
                ColumnId = i.ColumnId,
                Value = i.Value,
            }).ToList()
        };

        AddEvent($"RowEditPreview event: made a backup of record {row.Id}");
    }

    private void RowEditCancel(Row row)
    {
        // reset to original values
        foreach (var tableValue in row.TableValues)
            tableValue.Value = _rowBeforeEdit.TableValues.First(i => i.Id == tableValue.Id).Value;
        
        AddEvent($"RowEditCancel event: Editing of record: {row.Id} values: {string.Join(", ", row.TableValues.Select(i => i.Value))} canceled");
        StateHasChanged();
    }

    private async Task RowEditCommit(Row row)
    {
        foreach(var tableValue in row.TableValues)
            await DataServicesManager.McMasterService.UpdateTableValue(tableValue);

        AddEvent($"RowEditCommit event: Changes to record: {row.Id} values: {string.Join(", ", row.TableValues.Select(i => i.Value))} committed");
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
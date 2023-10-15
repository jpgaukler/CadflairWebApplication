using CadflairEntityFrameworkDataAccess.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Row = CadflairEntityFrameworkDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ManageProductDefinitions
{
    // services
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    // parameters
    [Parameter] public List<ProductDefinition> ProductDefinitions { get; set; } = new();

    // fields
    private ProductDefinition? _selectedProductDefinition;
    private ProductTable _productTable = new();
    private List<Row> _rows = new();
    private Row _newRow= new();
    private List<string> _events = new();


    private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
    private string _dragStyle = _initialDragStyle;

    private void ProductDefinition_OnClick(ProductDefinition productDefinition)
    {
        _selectedProductDefinition = productDefinition;
        //_productTable = DummyData.GetProductTableByProductDefinitionId(_selectedProductDefinition.Id);
        ResetNewRow_OnClick();

        // TO DO: load the product table
    }

    private void AddProductDefinition_OnClick()
    {
        ProductDefinitions.Add(new()
        {
            Name = "New product definition"
        });
    }

    private void AddColumn_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        _productTable.Columns.Add(new()
        {
            Id = _productTable.Columns.Count + 1,
            Header = "New Column",
        });

        ResetNewRow_OnClick();

        AddEvent($"Event = NewColumnDefinition");
    }

    private void ResetNewRow_OnClick()
    {
        if (_selectedProductDefinition == null) 
            return;
        
        _newRow = new()
        {
            TableValues = _productTable.Columns.Select(i => new TableValue()
            {
                ColumnId = i.Id,
            }).ToList()
        };

        //StateHasChanged();
    }

    private void AddRow_OnClick()
    {
        _rows.Add(_newRow);
        ResetNewRow_OnClick();

        AddEvent($"Event = NewProductRecord");
    }

    private void RowEditPreview(Row row)
    {
        // TO DO: create a backup copy of the item in memory
        //elementBeforeEdit = new()
        //{
        //    Sign = ((Element)element).Sign,
        //    Name = ((Element)element).Name,
        //    Molar = ((Element)element).Molar,
        //    Position = ((Element)element).Position
        //};

        AddEvent($"RowEditPreview event: made a backup of record {row.Id}");
        StateHasChanged();
    }

    private void RowEditCommit(Row row)
    {
        // TO DO: save changes to database

        AddEvent($"RowEditCommit event: Changes to record: {row.Id} values: {string.Join(", ", row.TableValues.Select(i => i.Value))} committed");
    }

    private void RowEditCancel(Row row)
    {
        // TO DO: reset to original values

        
        AddEvent($"RowEditCancel event: Editing of record: {row.Id} values: {string.Join(", ", row.TableValues.Select(i => i.Value))} canceled");
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
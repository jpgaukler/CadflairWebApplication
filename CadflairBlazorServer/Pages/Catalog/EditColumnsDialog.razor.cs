using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class EditColumnsDialog
{
    // services
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
    [Parameter] public User LoggedInUser { get; set; } = default!;
    [Parameter] public ProductTable ProductTable { get; set; } = default!;


    protected override void OnInitialized()
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
    }

    private async Task AddColumn_OnClick()
    {
        DialogResult result = await DialogService.Show<AddColumnDialog>("Add Column").Result;

        if (result.Canceled)
            return;

        AddColumnDialog dialog = (AddColumnDialog)result.Data;

        // check for duplicate column header
        if (ProductTable.Columns.Any(i => i.Header.Equals(dialog.Header, StringComparison.OrdinalIgnoreCase)))
        {
            Snackbar.Add("Header already used!", Severity.Warning);
            return;
        }

        Column newColumn = await DataServicesManager.CatalogService.CreateColumn(productTableId: ProductTable.Id,
                                                                                  header: dialog.Header,
                                                                                  sortOrder: ProductTable.Columns.Count + 1,
                                                                                  createdById: LoggedInUser.Id);

        // add an empty value to each row for the new column
        foreach (var row in ProductTable.Rows)
        {
            TableValue newTableValue = await DataServicesManager.CatalogService.CreateTableValue(productTableId: ProductTable.Id,
                                                                                                  rowId: row.Id,
                                                                                                  columnId: newColumn.Id,
                                                                                                  value: string.Empty,
                                                                                                  createdById: LoggedInUser.Id);
            row.TableValues.Add(newTableValue);
        }

        ProductTable.Columns.Add(newColumn);
    }

    //private async Task UpdateColumn_OnClick()
    //{

    //}

    private async Task IncreaseSortOrder_OnClick(Column column)
    {
        Column? nextColumn = ProductTable.Columns.FirstOrDefault(i => i.SortOrder == column.SortOrder + 1);

        if (nextColumn == null)
            return;

        nextColumn.SortOrder -= 1;
        column.SortOrder += 1;
        await DataServicesManager.CatalogService.UpdateColumn(nextColumn);
        await DataServicesManager.CatalogService.UpdateColumn(column);

        ProductTable.Columns.Sort();
    }

    private async Task DecreaseSortOrder_OnClick(Column column)
    {
        Column? previousColumn = ProductTable.Columns.FirstOrDefault(i => i.SortOrder == column.SortOrder - 1);

        if (previousColumn == null)
            return;

        previousColumn.SortOrder += 1;
        column.SortOrder -= 1;
        await DataServicesManager.CatalogService.UpdateColumn(previousColumn);
        await DataServicesManager.CatalogService.UpdateColumn(column);

        ProductTable.Columns.Sort();
    }

    private async Task DeleteColumn_OnClick(Column column)
    {
        foreach(var c in ProductTable.Columns.Where(i => i.SortOrder > column.SortOrder))
        {
            c.SortOrder -= 1;
            await DataServicesManager.CatalogService.UpdateColumn(c);
        }

        await DataServicesManager.CatalogService.DeleteColumnById(column.Id);
        ProductTable.Columns.Remove(column);
    }

    private void Ok_OnClick() => MudDialogInstance.Close();
}
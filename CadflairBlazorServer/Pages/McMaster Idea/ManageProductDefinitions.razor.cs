using Azure.Storage.Blobs;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Category = CadflairDataAccess.Models.Category;
using Column = CadflairDataAccess.Models.Column;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea;

public partial class ManageProductDefinitions
{
    // services
    [Inject] BlobServiceClient BlobServiceClient { get; set; } = default!;
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] ForgeServicesManager ForgeServicesManager { get; set; } = default!;
    [Inject] FileHandlingService FileHandlingService { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;
    [Inject] ILogger<ManageProductDefinitions> Logger { get; set; } = default!;

    // parameters
    [CascadingParameter] public User LoggedInUser { get; set; } = default!;
    [CascadingParameter] public Subscription Subscription { get; set; } = default!;
    [Parameter] public List<ProductDefinition> ProductDefinitions { get; set; } = new();

    // product data
    private ProductDefinition? _selectedProductDefinition;
    private Category? _category;
    private ProductTable? _productTable;
    private string? _searchString; 
    private Func<Row, bool> _searchFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.PartNumber.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.TableValues.Select(i => i.Value).Any(i => i.Contains(_searchString, StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    };

    // product table
    private Row _rowBeforeEdit = new();
    private bool _isDirty;
    private string _nameField = string.Empty;
    private string? _descriptionField; 
    private bool _importingExcel;

    // attachment uploads
    private class FileUpload
    {
        public IBrowserFile File { get; set; } = default!;
        public string Status { get; set; } = string.Empty;
    }
    private List<FileUpload> _fileUploads = new();
    private readonly string[] _validExtensions = { ".ipt", ".stp", ".step", ".pdf", ".dwg", ".idw" };
    private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
    private string _dragStyle = _initialDragStyle;

    private async Task ProductDefinition_OnClick(ProductDefinition? productDefinition)
    {
        _selectedProductDefinition = productDefinition;

        if (_selectedProductDefinition == null)
            return;

        _nameField = _selectedProductDefinition.Name;
        _descriptionField = _selectedProductDefinition.Description;
        _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_selectedProductDefinition.Id);
        _category = _selectedProductDefinition.CategoryId == null ? null : await DataServicesManager.McMasterService.GetCategoryById((int)_selectedProductDefinition.CategoryId);
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

    private async Task UpdateProductDefinition_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        if (_isDirty == false)
            return;

        if (string.IsNullOrWhiteSpace(_nameField))
            return;

        _selectedProductDefinition.Name = _nameField;
        _selectedProductDefinition.Description = _descriptionField;
        _selectedProductDefinition.CategoryId = _category?.Id;
        await DataServicesManager.McMasterService.UpdateProductDefinition(_selectedProductDefinition);
        _isDirty = false;

        Snackbar.Add("Changes saved!", Severity.Success);
    }

    private async Task UpdateCategory_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        DialogParameters parameters = new()
        {
            { nameof(SelectCategoryDialog.SubscriptionId), Subscription.Id },
        };

        DialogResult result = await DialogService.Show<SelectCategoryDialog>($"Select Category", parameters).Result;

        if (result.Canceled)
            return;

        SelectCategoryDialog dialog = (SelectCategoryDialog)result.Data;

        if (dialog.SelectedCategory?.Id == _selectedProductDefinition.CategoryId)
            return;

        _category = dialog.SelectedCategory;
        _isDirty = true;
    }

    private async Task DeleteProductDefinition_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        bool? confirmDelete = await DialogService.ShowMessageBox(title: "Delete Product Definition",
                                                                 message: "Are you sure you want to delete this Product Definition?",
                                                                 yesText: "Yes",
                                                                 cancelText: "Cancel");
        if (confirmDelete != true)
            return;

        // TO DO: need to delete the thumbnail from blob storage if there is one

        await DataServicesManager.McMasterService.DeleteProductDefinitionById(_selectedProductDefinition.Id);
        ProductDefinitions.Remove(_selectedProductDefinition);
        _selectedProductDefinition = null;
    }

    private async Task AddProductDefinition_OnClick()
    {
        DialogResult result = await DialogService.Show<AddProductDefinitionDialog>("Add Product Definition").Result;

        if (result.Canceled)
            return;


        // TO DO: CHECK FOR DUPLICATE PRODUCT DEFINITION NAME SO UNIQUE CONSTRAINT IS NOT VIOLATED

        AddProductDefinitionDialog dialog = (AddProductDefinitionDialog)result.Data;

        ProductDefinition newProductDefinition = await DataServicesManager.McMasterService.CreateProductDefinition(subscriptionId: Subscription.Id,
                                                                                                                   categoryId: null,
                                                                                                                   name: dialog.Name,
                                                                                                                   description: dialog.Description,
                                                                                                                   thumbnailUri: null,
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
                                                                         partNumber: dialog.PartNumber,
                                                                         createdById: LoggedInUser.Id);

        // add an new table value for each column 
        foreach (var tableValue in dialog.NewRowValues)
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

    private async Task ResetProductTable_OnClick()
    {
        if (_selectedProductDefinition == null)
            return;

        if (_productTable == null)
            return;

        bool? confirmDelete = await DialogService.ShowMessageBox(title: "Clear Product Table",
                                                                 message: "Are you sure you want to continue? All table data will be lost.",
                                                                 yesText: "Yes",
                                                                 cancelText: "Cancel");
        if (confirmDelete != true)
            return;

        await DataServicesManager.McMasterService.DeleteProductTableById(_productTable.Id);
        await DataServicesManager.McMasterService.CreateProductTable(_selectedProductDefinition.Id, LoggedInUser.Id);
        _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_selectedProductDefinition.Id);
    }

    private void RowEditPreview(Row row)
    {
        // create a backup copy of the values in memory
        _rowBeforeEdit = new()
        {
            PartNumber = row.PartNumber,
            TableValues = row.TableValues.Select(i => new TableValue()
            {
                Id = i.Id,
                RowId = i.RowId,
                ColumnId = i.ColumnId,
                Value = i.Value,
            }).ToList()
        };

        AddEvent($"RowEditPreview event: made a backup of record {row.PartNumber}");
    }

    private void RowEditCancel(Row row)
    {
        // reset to original values
        row.PartNumber = _rowBeforeEdit.PartNumber;

        foreach (var tableValue in row.TableValues)
            tableValue.Value = _rowBeforeEdit.TableValues.First(i => i.Id == tableValue.Id).Value;

        AddEvent($"RowEditCancel event: Editing of record: {row.PartNumber} values: {string.Join(", ", row.TableValues.Select(i => i.Value))} canceled");
        StateHasChanged();
    }

    private async Task RowEditCommit(Row row)
    {
        await DataServicesManager.McMasterService.UpdateRow(row);
        AddEvent($"RowEditCommit event: Changes to record: {row.PartNumber} values: {string.Join(", ", row.TableValues.Select(i => i.Value))} committed");
    }

    private async Task ImportExcel_OnClick(IBrowserFile browserFile)
    {
        if (_productTable == null)
            return;

        string[] validExtensions = { ".xlsx", ".xls"};

        if (!validExtensions.Any(i => i == Path.GetExtension(browserFile.Name)))
        {
            Snackbar.Add("Invalid file extension!", Severity.Warning);
            return;
        }

        _importingExcel = true;
        string? tempFilename = null;

        try
        {
            tempFilename = await FileHandlingService.UploadBrowserFileToTempFolder(browserFile);

            XLWorkbook workbook;
            using (FileStream fs = File.Open(tempFilename, FileMode.Open, FileAccess.Read))
            {
                workbook = new(fs);
            }

            var worksheet = workbook.Worksheets.First();
            int partNumberColumnIndex = 1;

            Dictionary<int, int> columnMappings = new();

            // add columns
            foreach (var xlCell in worksheet.FirstRowUsed().Cells())
            {
                int columnIndex = xlCell.Address.ColumnNumber;
                string? columnHeader = xlCell.Value.ToString();

                if (string.IsNullOrWhiteSpace(columnHeader))
                    break;

                if (columnHeader.Contains("Part Number", StringComparison.OrdinalIgnoreCase))
                {
                    partNumberColumnIndex = columnIndex;
                    continue;
                }

                Column newColumn = await DataServicesManager.McMasterService.CreateColumn(productTableId: _productTable.Id,
                                                                                          header: columnHeader,
                                                                                          sortOrder: _productTable.Columns.Count + 1,
                                                                                          createdById: LoggedInUser.Id);
                columnMappings.Add(columnIndex, newColumn.Id);
                _productTable.Columns.Add(newColumn);
            }

            // add rows
            foreach (var xlRow in worksheet.RowsUsed())
            {
                // skip the header row
                if (xlRow == worksheet.FirstRowUsed())
                    continue;

                string? partNumber = xlRow.Cell(partNumberColumnIndex).Value.ToString();

                if (string.IsNullOrWhiteSpace(partNumber))
                    break;

                Row newRow = await DataServicesManager.McMasterService.CreateRow(productTableId: _productTable.Id,
                                                                                 partNumber: partNumber,
                                                                                 createdById: LoggedInUser.Id);


                // add an new table value for each column 
                foreach (var columnMapping in columnMappings)
                {
                    int columnId = columnMapping.Value;
                    string value = xlRow.Cell(columnMapping.Key).Value.ToString();

                    TableValue newTableValue = await DataServicesManager.McMasterService.CreateTableValue(productTableId: _productTable.Id,
                                                                                                          rowId: newRow.Id,
                                                                                                          columnId: columnId,
                                                                                                          value: value,
                                                                                                          createdById: LoggedInUser.Id);

                    newRow.TableValues.Add(newTableValue);
                }

                _productTable.Rows.Add(newRow);
            }


            Snackbar.Add("Data imported successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Import Excel data failed!");
            Snackbar.Add("Data import failed!", Severity.Error);
        }

        // clean up
        if (tempFilename != null)
            File.Delete(tempFilename);

        _importingExcel = false;
    }

    private List<string> _events = new();
    private void AddEvent(string message)
    {
        _events.Insert(0, message);
        StateHasChanged();
    }

    private async Task UpdateThumbnail(string? thumbnailUri)
    {
        if (_selectedProductDefinition == null)
            return;

        _selectedProductDefinition.ThumbnailUri = thumbnailUri;
        await DataServicesManager.McMasterService.UpdateProductDefinition(_selectedProductDefinition);

        Snackbar.Add($"Thumbnail updated!", Severity.Success);
    }

    private async Task Attachments_FilesChanged(IReadOnlyList<IBrowserFile> files)
    {
        if (_selectedProductDefinition == null)
            return;

        // create bucket for CAD files
        if (string.IsNullOrWhiteSpace(_selectedProductDefinition.ForgeBucketKey))
        {
            _selectedProductDefinition.ForgeBucketKey = Guid.NewGuid().ToString();
            await ForgeServicesManager.ObjectStorageService.CreateBucket(_selectedProductDefinition.ForgeBucketKey);
            await DataServicesManager.McMasterService.UpdateProductDefinition(_selectedProductDefinition);
        }

        _fileUploads = files.Select(file => new FileUpload
        {
            File = file,
            Status = "Not started",
        }).ToList();


        foreach (var upload in _fileUploads)
        {
            string? tempFilename = null;

            IProgress<string> progress = new Progress<string>(message =>
            {
                upload.Status = message;
                StateHasChanged();
            });

            if (!_validExtensions.Any(i => i == Path.GetExtension(upload.File.Name)))
            {
                progress.Report("Invalid file extension");
                continue;
            }

            // find row to link this file to
            Row? matchingRow = _productTable?.Rows.FirstOrDefault(i => i.PartNumber.Equals(Path.GetFileNameWithoutExtension(upload.File.Name), StringComparison.OrdinalIgnoreCase));

            if (matchingRow == null)
            {
                progress.Report("Part number not found");
                continue;
            }

            // check for duplicate attachment
            string objectKey = upload.File.Name;
            if (matchingRow.Attachments.Any(i => i.ForgeObjectKey.Equals(objectKey, StringComparison.OrdinalIgnoreCase)))
            {
                progress.Report("Invalid file name");
                continue;
            }

            progress.Report("Processing");

            try
            {
                // save file to server
                tempFilename = await FileHandlingService.UploadBrowserFileToTempFolder(upload.File);

                bool uploadSuccessful = await ForgeServicesManager.ObjectStorageService.UploadFile(_selectedProductDefinition.ForgeBucketKey, objectKey, tempFilename);

                if (!uploadSuccessful)
                    throw new Exception("Could not upload to Autodesk OSS!");

                // start the model translation
                await ForgeServicesManager.ModelDerivativeService.TranslateObject(_selectedProductDefinition.ForgeBucketKey, objectKey, false, null);

                // create database record
                Attachment attachment = await DataServicesManager.McMasterService.CreateAttachment(rowId: matchingRow.Id, forgeObjectKey: objectKey, createdById: LoggedInUser.Id);
                matchingRow.Attachments.Add(attachment);

                progress.Report("Success");
            }
            catch (Exception ex)
            {
                progress.Report("Error");
                Logger.LogError(ex, $"Error occurred while creating CatalogModel!");
            }

            // clean up
            if (tempFilename != null)
                File.Delete(tempFilename);
        }
    }


    private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
    private void ClearDragStyle() => _dragStyle = _initialDragStyle;

}
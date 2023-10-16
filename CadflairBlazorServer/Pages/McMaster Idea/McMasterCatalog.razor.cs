using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Row = CadflairEntityFrameworkDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class McMasterCatalog
    {
        // services
        [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] ILogger<MyCatalog> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private bool _drawerOpen = true;
        private bool _initializing = true;
        private bool _loadingCatalogModels = false;
        private List<Category> _categories = new();
        private Category? _selectedCategory;
        private List<ProductDefinition> _productDefinitions = new();
        private ProductDefinition? _selectedProductDefinition;
        private ProductTable _productTable = new();
        private List<Row> _rows = new();
        private Dictionary<int, HashSet<string>> _columnFilters = new();

        private Func<Row, bool> _filter => row =>
        {
            if (_selectedProductDefinition == null)
                return true;

            //foreach(var column in _productTable.Columns)
            //{
            //    // if no filter value selected then continue to next column
            //    if (_columnFilters.ContainsKey(column.Id) == false)
            //        continue;

            //    // if one or more filter values selected, then exclude products that do not match visible values
            //    TableValue value = row.TableValues.First(i => i.ColumnId == column.Id);

            //    // check to see if this value is selected in the column filters
            //    if (_columnFilters[column.Id].Contains(value.Value) == false)
            //        return false;
            //}

            return true;
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await Task.Delay(2000);
                //_categories = DummyData.GetProductCategories();

                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private void Category_OnClick(Category? selectedCategory)
        {
            _selectedCategory = selectedCategory;
            _selectedProductDefinition = null;
        }

        private void ProductDefinition_OnClick(ProductDefinition? selectedDefinition)
        {
            _selectedProductDefinition = selectedDefinition;
            _columnFilters.Clear();

            if (_selectedProductDefinition == null)
                return;

            //_products = DummyData.GetProductByProductDefinitionId(_selectedProductDefinition.Id);
        }

        private void ColumnFilter_OnSelect(Column column, IEnumerable<string> values)
        {
            if (_columnFilters.ContainsKey(column.Id))
                _columnFilters.Remove(column.Id);

            if (!values.Any())
                return;

            _columnFilters.Add(column.Id, values.ToHashSet());
        }

        private void Preview_OnClick(string bucketKey, string objectKey)
        {
            DialogParameters parameters = new()
            {
                { nameof(PreviewDialog.BucketKey), bucketKey },
                { nameof(PreviewDialog.ObjectKey), objectKey },
            };

            DialogService.Show<PreviewDialog>($"Preview", parameters);
        }

        private async Task Download_OnClick(string bucketKey, string objectKey)
        {
            string url = await ForgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(bucketKey, objectKey);
            NavigationManager.NavigateTo(url);
        }

    }
}
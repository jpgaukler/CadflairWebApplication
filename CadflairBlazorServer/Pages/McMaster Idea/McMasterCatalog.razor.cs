using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

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
        private List<ProductCategory> _productCategories = new();
        private ProductCategory? _selectedProductCategory;
        private ProductDefinition? _selectedProductDefinition;
        private List<Product> _products = new();
        private Dictionary<ColumnDefinition, HashSet<string>> _columnFilters = new();

        private Func<Product, bool> _filter => product =>
        {
            if (_selectedProductDefinition == null)
                return true;

            foreach(var columnDefinition in _selectedProductDefinition.ColumnDefinitions)
            {
                // if no filter value selected then continue to next column
                if (_columnFilters.ContainsKey(columnDefinition) == false)
                    continue;

                // if one or more filter values selected, then exclude products that do not match visible values
                ColumnValue columnValue = product.ColumnValues.First(i => i.ColumnDefinitionId == columnDefinition.Id);

                // I WILL PROBABLY WANT TO UPDATE THIS TO LOOK AT THE COLUMNVALUE.ID RATHER THAN THE STRING VALUE
                if (_columnFilters[columnDefinition].Contains(columnValue.Value) == false)
                    return false;
            }

            return true;
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await Task.Delay(2000);
                _productCategories = DummyData.GetProductCategories();

                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private void ProductCategory_OnClick(ProductCategory? selectedCategory)
        {
            _selectedProductCategory = selectedCategory;
            _selectedProductDefinition = null;
        }

        private void ProductDefinition_OnClick(ProductDefinition? selectedDefinition)
        {
            _selectedProductDefinition = selectedDefinition;
            _columnFilters.Clear();

            if (_selectedProductDefinition == null)
                return;

            _products = DummyData.GetProductByProductDefinitionId(_selectedProductDefinition.Id);
        }

        private void ColumnFilter_OnSelect(ColumnDefinition columnDefinition, IEnumerable<string> values)
        {
            if (_columnFilters.ContainsKey(columnDefinition))
                _columnFilters.Remove(columnDefinition);

            if (!values.Any())
                return;

            _columnFilters.Add(columnDefinition, values.ToHashSet());
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
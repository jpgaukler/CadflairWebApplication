using Microsoft.AspNetCore.Components;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class Products
    {
        // services
        [Inject] DataServicesManager  DataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] ILogger<Categories> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;
        [Parameter] public string ProductDefinitionName { get; set; } = string.Empty;

        // fields
        private Subscription? _subscription;
        private ProductDefinition? _productDefinition;
        private Row? _selectedRow;
        private ProductTable _productTable = new();
        private Dictionary<int, HashSet<string>> _columnFilters = new();
        private bool _mobileFilterOpen = false;
        private bool _initializing = true;
        private string _mobileFilterStyle = "top:0; transform: translateY(calc(100% - 1.5rem - 24px)); transition: 225ms cubic-bezier(0,0,.2,1) 0ms;";

        private Func<Row, bool> _filter => row =>
        {
            foreach (var column in _productTable.Columns)
            {
                // if no filter value selected then continue to next column
                if (_columnFilters.ContainsKey(column.Id) == false)
                    continue;

                // if one or more filter values selected, then exclude products that do not match visible values
                TableValue value = row.TableValues.First(i => i.ColumnId == column.Id);

                // check to see if this value is selected in the column filters
                if (_columnFilters[column.Id].Contains(value.Value) == false)
                    return false;
            }

            return true;
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
                _productDefinition = await DataServicesManager.McMasterService.GetProductDefinitionByNameAndSubscriptionId(ProductDefinitionName, _subscription.Id);
                _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_productDefinition.Id);
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing products page!");
                NavigationManager.NavigateTo("/notfound");
                return;
            }
        }

        private void ColumnFilter_OnSelect(Column column, IEnumerable<string> values)
        {
            if (_columnFilters.ContainsKey(column.Id))
                _columnFilters.Remove(column.Id);

            if (!values.Any())
                return;

            _columnFilters.Add(column.Id, values.ToHashSet());
        }

        private void RowLink_OnClick(Row row)
        {
            NavigationManager.NavigateTo($"{CompanyName}/products/{ProductDefinitionName}/{row.PartNumber}/");
        }

        private void ToggleMobileFilter_OnClick()
        {
            _mobileFilterOpen = !_mobileFilterOpen;

            if (_mobileFilterOpen)
                _mobileFilterStyle = "top:0; transform: translateY(0px); transition: 225ms cubic-bezier(0,0,.2,1) 0ms; padding-top: calc(var(--mud-appbar-height) - var(--mud-appbar-height)/8); z-index:1200;";
            else
                _mobileFilterStyle = "top:0; transform: translateY(calc(100% - 1.5rem - 24px)); transition: 225ms cubic-bezier(0,0,.2,1) 0ms; z-index:1200;";
        }

    }
}
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class McMasterCatalogEditor
    {
        // services
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] ILogger<MyCatalog> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private bool _drawerOpen = true;
        private bool _initializing = true;
        private string _newColumnHeader = string.Empty;
        private ProductDefinition _productDefinition = new();
        private List<ProductCategory> _productCategories = new();
        private List<Product> _products = new();
        private List<string> _events = new();

        private Product _newProduct= new();


        protected override async Task OnInitializedAsync()
        {
            try
            {
                await Task.Delay(1000);
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private void AddColumnDefinition_OnClick()
        {
            if (string.IsNullOrWhiteSpace(_newColumnHeader))
                return;

            _productDefinition.ColumnDefinitions.Add(new()
            {
                Id = _productDefinition.ColumnDefinitions.Count + 1,
                Header = _newColumnHeader,
            });

            ResetNewProduct_OnClick();

            AddEvent($"Event = NewColumnDefinition");

            _newColumnHeader = string.Empty;
        }

        private void ResetNewProduct_OnClick()
        {
            _newProduct = new()
            {
                ColumnValues = _productDefinition.ColumnDefinitions.Select(i => new ColumnValue()
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

        //private void StartedEditingItem(string record)
        //{
        //    _dataGridEvents.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(record)}");
        //}

        //private void CanceledEditingItem(string record)
        //{
        //    _dataGridEvents.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(record)}");
        //}

        //private void CommittedItemChanges(string record)
        //{
        //    _dataGridEvents.Insert(0, $"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(record)}");
        //}

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

    }
}
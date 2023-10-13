using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

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

        private List<ProductCategory> _productCategories = new();
        private ProductCategory? _selectedProductCategory;

        private List<ProductDefinition> _productDefinitions = new();
        private ProductDefinition? _selectedProductDefinition;

        private ProductDefinition _productDefinition = new();
        private List<Product> _products = new();
        private List<string> _events = new();

        private Product _newProduct= new();


        private string _dragStyle = string.Empty;
        private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
        private void ClearDragStyle() => _dragStyle = string.Empty;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                _productCategories = DummyData.GetProductCategories();
                _productDefinitions = DummyData.GetProductDefinitions();
                await Task.Delay(1000);
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private void AddProductCategory_OnClick(ProductCategory? parentCategory)
        {
            ProductCategory newCategory = new()
            {
                Name = "New Category",
            };

            if (parentCategory == null)
                _productCategories.Add(newCategory);
            else
                parentCategory.ChildCategories.Add(newCategory);
        }

        private void DeleteProductCategory_OnClick(ProductCategory? parentCategory)
        {
            // TO DO: delete product category and update linked entities (other categories and product definitions)
        }

        private void MoveProductCategory_OnClick(ProductCategory? parentCategory)
        {
            // TO DO: change parent id for the category
        }

        private void AddProductDefinition_OnClick()
        {
            _productDefinitions.Add(new()
            {
                Name = "New product definition"
            });
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

        private void UploadThumbnailImage(IBrowserFile file)
        {
            //TODO upload the files to the server
        }


    }
}
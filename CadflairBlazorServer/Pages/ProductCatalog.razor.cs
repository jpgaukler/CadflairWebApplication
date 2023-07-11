using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CadflairBlazorServer.Pages
{
    public partial class ProductCatalog
    {
        // services

        [Inject] ProtectedSessionStorage _protectedSessionStorage { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private Subscription? _subscription;
        private List<Product> _products = new();
        private List<BreadcrumbItem> _breadcrumbItems = new();
        private bool _displayListView = false;
        //private bool _showDetails = false;
        private bool _drawerOpen = true;
        private bool _initializing = true;
        private DialogOptions _productFolderDialogOptions = new() 
        { 
            FullWidth = true, 
            MaxWidth = MaxWidth.ExtraSmall, 
            DisableBackdropClick = true 
        };
        private bool _showNewFolderDialog = false;
        private string? _newProductFolderName;

        // fields
        private List<ProductFolder> _productFolders = new List<ProductFolder>();
        private ProductFolder? _selectedProductFolder;

        protected override async Task OnInitializedAsync()
        {
            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);

            if (_subscription == null)
            {
                _navigationManager.NavigateTo("/notfound");
                return;
            }

            await LoadProductFolders();
            await SelectedProductFolderChanged(_productFolders.FirstOrDefault());
            _initializing = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // load view setting
                var viewSetting = await _protectedSessionStorage.GetAsync<bool>("_displayListView");
                _displayListView = viewSetting.Success ? viewSetting.Value : false;
                StateHasChanged();
            }
        }

        private async Task LoadProductFolders()
        {
            _productFolders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionId(_subscription!.Id);
        }

        private async Task SelectedProductFolderChanged(ProductFolder? selectedFolder)
        {
            _selectedProductFolder = selectedFolder;

            if (_selectedProductFolder == null)
            {
                _products.Clear();
                //_breadcrumbItems.Clear();
                return;
            }

            ProductFolder folder = _selectedProductFolder;
            _products = (await _dataServicesManager.ProductService.GetProductsByProductFolderId(folder.Id))
                                                                  .Where(i => i.IsPublic)
                                                                  .ToList();

            //// refresh breadcrumbs
            //_breadcrumbItems.Clear();
            //_breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null, disabled: true));

            //while (folder.ParentId != null)
            //{
            //    folder = await _dataServicesManager.ProductService.GetProductFolderById((int)folder.ParentId);
            //    _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null, disabled: true));
            //}

            //// reverse the list so the breadcrumbs are displayed from the top down
            //_breadcrumbItems.Reverse();
        }

        private async Task AddNewProductFolder_OnClick()
        {
            await Task.Delay(1000);

            if (_newProductFolderName == null)
                return;

            //ProductFolder newFolder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId: _subscription!.Id,
            //                                                                                        createdById: 1,
            //                                                                                        displayName: _newProductFolderName,
            //                                                                                        parentId: _selectedProductFolder?.Id);

            //if (_selectedProductFolder == null)
            //{
            //    _productFolders.Add(newFolder);
            //    _productFolders.Sort();
            //}
            //else
            //{
            //    _selectedProductFolder.ChildFolders.Add(newFolder);
            //    _selectedProductFolder.ChildFolders.Sort();
            //}

            //_newProductFolderName = null;
            //_showNewFolderDialog = false;
        }

        private void CancelNewProductFolder_OnClick()
        {
            _newProductFolderName = null;
            _showNewFolderDialog = false;
        }


        private void ProductsGrid_OnRowClick(DataGridRowClickEventArgs<Product> args)
        {
            Product product = args.Item;
            _navigationManager.NavigateTo($"/{_subscription!.SubdirectoryName}/products/{product.SubdirectoryName}");
        }

        private async Task ToggleView_OnClick()
        {
            _displayListView = !_displayListView;
            await _protectedSessionStorage.SetAsync("_displayListView", _displayListView);
        }
    }
}
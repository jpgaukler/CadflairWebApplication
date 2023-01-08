using CadflairDataAccess.Models;
using Microsoft.AspNetCore.Components;
using CadflairDataAccess;
using MudBlazor;
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
        private Subscription _subscription = default!;
        private List<Product> _products = new();
        private List<BreadcrumbItem> _breadcrumbItems = new();
        private bool _displayListView = true;
        private bool _showDetails = false;
        private bool _loadingData = true;

        // class for product folder tree structure
        private class ProductFolderTreeItem
        {
            public ProductFolder ProductFolder { get; set; } = new();
            public HashSet<ProductFolderTreeItem> ChildItems { get; set; } = new();
        }

        // fields
        private ProductFolderTreeItem? _selectedTreeItem = default!;
        private HashSet<ProductFolderTreeItem> _productFolderTreeItems = new();

        protected override async Task OnInitializedAsync()
        {
            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);

            if (_subscription == null)
            {
                _navigationManager.NavigateTo("/notfound");
            }

            await LoadProductFoldersRecursive(null, _productFolderTreeItems);
            await SelectedTreeItemChanged(_productFolderTreeItems.FirstOrDefault());
            _loadingData = false;
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

        private async Task LoadProductFoldersRecursive(int? parentId, HashSet<ProductFolderTreeItem> treeItems)
        {
            List<ProductFolder> folders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionIdAndParentId(_subscription.Id, parentId);

            foreach (ProductFolder folder in folders)
            {
                var treeItem = new ProductFolderTreeItem() { ProductFolder = folder };
                treeItems.Add(treeItem);
                await LoadProductFoldersRecursive(folder.Id, treeItem.ChildItems);
            }
        }

        private async Task SelectedTreeItemChanged(ProductFolderTreeItem? selectedItem)
        {
            _selectedTreeItem = selectedItem;

            if (_selectedTreeItem == null)
            {
                _products.Clear();
                _breadcrumbItems.Clear();
                return;
            }

            ProductFolder folder = _selectedTreeItem.ProductFolder;
            _products = (await _dataServicesManager.ProductService.GetProductsByProductFolderId(folder.Id)).Where(i => i.IsPublic).ToList();

            // refresh breadcrumbs
            _breadcrumbItems.Clear();
            _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null, disabled: true));

            while (folder.ParentId != null)
            {
                folder = await _dataServicesManager.ProductService.GetProductFolderById((int)folder.ParentId);
                _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null, disabled: true));
            }

            // reverse the list so the breadcrumbs are displayed from the top down
            _breadcrumbItems.Reverse();
        }

        private void ProductsGrid_OnRowClick(DataGridRowClickEventArgs<Product> args)
        {
            Product product = args.Item;
            _navigationManager.NavigateTo($" /{_subscription.SubdirectoryName}/products/{product.SubdirectoryName}");
        }

        private async Task ToggleView()
        {
            _displayListView = !_displayListView;
            await _protectedSessionStorage.SetAsync("_displayListView", _displayListView);
        }
    }
}
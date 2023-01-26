using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairBlazorServer.Helpers;
using CadflairBlazorServer.Shared.Dialogs;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ProductFolderTree
    {
        // services
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
        [Inject] DataServicesManager  _dataServicesManager { get; set; } = default!;
        [Inject] IDialogService _dialogService  {get; set; } = default!;
        [Inject] ISnackbar  _snackbar { get; set; } = default!;

        //parameters

        /// <summary>
        /// Returns the folder that was last selected.
        /// </summary>
        [Parameter]
        public ProductFolder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (_selectedFolder?.Id == value?.Id) return;
                _selectedFolder = value;
                SelectedFolderChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Callback that is called when a folder selected.
        /// </summary>
        [Parameter] public EventCallback<ProductFolder> SelectedFolderChanged { get; set; }

        // fields
        private User _loggedInUser = new();
        private HashSet<ProductFolderTreeItem> _productFolderTreeItems = new();
        private ProductFolderTreeItem? _selectedFolderTreeItem;
        private ProductFolder? _selectedFolder;

        // class for product folder tree structure
        private class ProductFolderTreeItem
        {
            public ProductFolder ProductFolder { get; set; } = new();
            public HashSet<ProductFolderTreeItem> ChildItems { get; set; } = new();
            public bool IsExpanded { get; set; } = false;
            public bool HasChild => ChildItems != null && ChildItems.Count > 0;
        }


        protected override async Task OnInitializedAsync()
        {
            // get data
            _loggedInUser = await _authenticationStateProvider.GetUser(_dataServicesManager);
            if (_loggedInUser.SubscriptionId == null) return;

            // load the root level product folders
            await LoadProductFoldersRecursive(null, _productFolderTreeItems);
        }

        private async Task LoadProductFoldersRecursive(int? parentId, HashSet<ProductFolderTreeItem> treeItems)
        {
            List<ProductFolder> folders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionIdAndParentId((int)_loggedInUser.SubscriptionId!, parentId);

            foreach (ProductFolder folder in folders)
            {
                var treeItem = new ProductFolderTreeItem() { ProductFolder = folder };
                treeItems.Add(treeItem);
                await LoadProductFoldersRecursive(folder.Id, treeItem.ChildItems);
            }
        }

        private void SelectedTreeItemChanged(ProductFolderTreeItem? selectedItem)
        {
            _selectedFolderTreeItem = selectedItem;
            SelectedFolder = selectedItem?.ProductFolder;
        }

        private async Task NewProductFolder_OnClick()
        {
            var result = await _dialogService.Show<ProductFolderDialog>("New Folder").Result;

            if (!result.Canceled)
            {
                string newFolderName = (string)result.Data;

                List<ProductFolder> existingFolders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionIdAndParentId((int)_loggedInUser.SubscriptionId, _selectedFolder?.Id);
                if (existingFolders.Exists(i => i.DisplayName.Equals(newFolderName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    _snackbar.Add("A folder with this name already exists.", Severity.Warning);
                    return;
                }

                ProductFolder productFolder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId: (int)_loggedInUser.SubscriptionId, 
                                                                                                            createdById: _loggedInUser.Id, 
                                                                                                            displayName: newFolderName, 
                                                                                                            parentId: _selectedFolder?.Id);

                var treeItem = new ProductFolderTreeItem() { ProductFolder = productFolder };

                if (_selectedFolderTreeItem == null)
                {
                    _productFolderTreeItems.Add(treeItem);
                    _productFolderTreeItems = _productFolderTreeItems.OrderBy(i => i.ProductFolder.DisplayName).ToHashSet();
                }
                else
                {
                    _selectedFolderTreeItem.ChildItems.Add(treeItem);
                    _selectedFolderTreeItem.ChildItems = _selectedFolderTreeItem.ChildItems.OrderBy(i => i.ProductFolder.DisplayName).ToHashSet();
                }

                _snackbar.Add("Folder created.", Severity.Success);
            }
        }

        private async Task DeleteProductFolderTreeItem_OnClick(ProductFolderTreeItem treeItem)
        {
            if (treeItem == null) return;

            bool? result = await _dialogService.ShowMessageBox(title: "Delete Folder",
                                                               message: "Are you sure you want to delete this folder?",
                                                               yesText: "Yes",
                                                               cancelText: "Cancel");

            if (result == true)
            {
                await _dataServicesManager.ProductService.DeleteProductFolder(treeItem.ProductFolder);
                RemoveProductFolderTreeItem(treeItem);
                _selectedFolder = null;

                _snackbar.Add("Folder deleted.", Severity.Success);
            }
        }

        private void RemoveProductFolderTreeItem(ProductFolderTreeItem itemToRemove, HashSet<ProductFolderTreeItem>? items = null)
        {
            items ??= _productFolderTreeItems;

            foreach (ProductFolderTreeItem item in items)
            {
                if (items.Remove(itemToRemove)) return;
                RemoveProductFolderTreeItem(itemToRemove, item.ChildItems);
            }
        }
    }
}
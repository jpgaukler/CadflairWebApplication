using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairBlazorServer.Helpers;

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
            _loggedInUser = await _authenticationStateProvider.GetUser(_dataServicesManager.UserService);
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
            DialogParameters parameters = new()
            {
                { "ParentId", _selectedFolder?.Id },
                { "UserId", _loggedInUser.Id },
                { "SubscriptionId", _loggedInUser.SubscriptionId }
            };

            var result = await _dialogService.Show<ProductFolderDialog>("New Folder", parameters).Result;

            if (!result.Canceled)
            {
                var treeItem = new ProductFolderTreeItem() { ProductFolder = (ProductFolder)result.Data };

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
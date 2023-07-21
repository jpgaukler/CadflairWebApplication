using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CadflairBlazorServer.Pages
{
    public partial class CadModelCatalog
    {
        // services

        [Inject] AuthenticationService _authenticationService { get; set; } = default!;
        [Inject] ProtectedSessionStorage _protectedSessionStorage { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;
        [Inject] FileHandlingService _fileHandlingService { get; set; } = default!;
        [Inject] ForgeServicesManager _forgeServicesManager { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private Subscription? _subscription;
        private ProductFolder? _selectedProductFolder;
        private List<ProductFolder> _productFolders = new();
        private List<BreadcrumbItem> _breadcrumbItems = new();
        private List<CadModel> _cadModels = new();
        private bool _displayListView = false;
        private string _displayListViewSettingKey = "EFAE9570-C7C5-45FC-BF26-86E69F8DB677";
        private bool _drawerOpen = true;
        private bool _initializing = true;

        // new folder dialog
        private DialogOptions _newProductFolderDialogOptions = new() 
        { 
            FullWidth = true, 
            MaxWidth = MaxWidth.ExtraSmall, 
            DisableBackdropClick = true,
            CloseOnEscapeKey = false
        };
        private bool _showNewProductFolderDialog = false;
        private string? _newProductFolderName;

        // new cad model dialog
        private DialogOptions _newCadModelDialogOptions = new() 
        { 
            FullWidth = true, 
            MaxWidth = MaxWidth.Medium, 
            DisableBackdropClick = true,
            CloseOnEscapeKey = false
        };
        private bool _showNewCadModelDialog = false;
        private string? _newCadModelDisplayName;
        private string? _newCadModelDescription;
        private bool _newCadModelValid = false;
        private string _dragStyle = string.Empty;
        private IBrowserFile? _attachedFile;
        private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
        private void ClearDragStyle() => _dragStyle = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);

            if (_subscription == null)
            {
                _navigationManager.NavigateTo("/notfound");
                return;
            }

            _productFolders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionId(_subscription!.Id);
            _initializing = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // load view setting
                var viewSetting = await _protectedSessionStorage.GetAsync<bool>(_displayListViewSettingKey);
                _displayListView = viewSetting.Success ? viewSetting.Value : false;
                StateHasChanged();
            }
        }

        private async Task ProductFolder_OnClick(ProductFolder selectedFolder)
        {
            _cadModels.Clear();
            _breadcrumbItems.Clear();

            if (_selectedProductFolder == selectedFolder)
            {
                _selectedProductFolder = null;
                return;
            }

            _selectedProductFolder = selectedFolder;
            _cadModels = await _dataServicesManager.CadModelService.GetCadModelsByProductFolderId(_selectedProductFolder.Id);

            // refresh breadcrumbs
            ProductFolder folder = _selectedProductFolder;

            _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null));

            while (folder.ParentFolder != null)
            {
                folder = folder.ParentFolder;
                _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null));
            }

            // reverse the list so the breadcrumbs are displayed from the top down
            _breadcrumbItems.Reverse();
        }

        private async Task BreadcrumbItem_OnClick(BreadcrumbItem breadcrumbItem)
        {
            if (_selectedProductFolder == null)
                return;

            ProductFolder folder = _selectedProductFolder;

            while (folder.ParentFolder != null)
            {
                folder = folder.ParentFolder;

                if (folder.DisplayName == breadcrumbItem.Text)
                {
                    await ProductFolder_OnClick(folder);
                    break;
                }
            }
        }

        private void CadModelsGrid_OnRowClick(DataGridRowClickEventArgs<CadModel> args)
        {
            _navigationManager.NavigateTo($"catalog/{_subscription!.SubdirectoryName}/{args.Item.Guid}");
        }

        private async Task ToggleView_OnClick()
        {
            _displayListView = !_displayListView;
            await _protectedSessionStorage.SetAsync(_displayListViewSettingKey, _displayListView);
        }

        private async Task NewProductFolder_OnSubmit()
        {
            await Task.Delay(1000);

            if (_newProductFolderName == null)
                return;

            ProductFolder newFolder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId: _subscription!.Id,
                                                                                                    createdById: 1,
                                                                                                    displayName: _newProductFolderName,
                                                                                                    parentId: _selectedProductFolder?.Id);

            if (_selectedProductFolder == null)
            {
                _productFolders.Add(newFolder);
                _productFolders.Sort();
            }
            else
            {
                _selectedProductFolder.ChildFolders.Add(newFolder);
                _selectedProductFolder.ChildFolders.Sort();
            }

            NewProductFolder_OnClose();
        }

        private void NewProductFolder_OnClose()
        {
            _newProductFolderName = null;
            _showNewProductFolderDialog = false;
        }

        private async Task NewCadModel_OnSubmit()
        {
            if (await _authenticationService.IsLoggedInUserValid() == false)
            {
                _navigationManager.NavigateTo("/notauthorized");
                return;
            }

            if (!_newCadModelValid)
                return;

            if (_attachedFile == null)
                return;

            if (_selectedProductFolder == null)
                return;

            string? tempFilename = null;

            try
            {
                //int uploadProgress = 0;
                //Progress<int> progress = new(value =>
                //{
                //    // update a progress field here if desired
                //    uploadProgress = value;
                //    StateHasChanged();
                //});

                // save file to server
                tempFilename = await _fileHandlingService.UploadBrowserFileToTempFolder(_attachedFile);

                // upload file to Autodesk OSS
                string bucketKey = Guid.NewGuid().ToString();
                await _forgeServicesManager.ObjectStorageService.CreateBucket(bucketKey);

                string objectKey = Guid.NewGuid().ToString();
                bool uploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, objectKey, tempFilename);

                if (!uploadSuccessful)
                    throw new Exception("Could not upload to Autodesk OSS!");

                // create database record
                CadModel cadModel = await _dataServicesManager.CadModelService.CreateCadModel(subscriptionId: _subscription!.Id,
                                                                                              productFolderId: _selectedProductFolder.Id,
                                                                                              createdById: _authenticationService.LoggedInUser!.Id,
                                                                                              displayName: _newCadModelDisplayName,
                                                                                              description: _newCadModelDescription,
                                                                                              bucketKey: bucketKey,
                                                                                              objectKey: objectKey);

                // start the model translation
                //await _forgeServicesManager.ModelDerivativeService.TranslateObject();

            }
            catch (Exception ex)
            {
                _snackbar.Add("An error occurred!", Severity.Error);
                Debug.WriteLine(ex);
            }

            // clean up
            if (tempFilename != null)
                File.Delete(tempFilename);

            NewCadModel_OnClose();
        }

        private void NewCadModel_OnClose()
        {
            _newCadModelDisplayName = null;
            _newCadModelDescription = null;
            _attachedFile = null;
            _showNewCadModelDialog = false;
        }

    }
}
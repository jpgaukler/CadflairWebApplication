using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace CadflairBlazorServer.Pages
{
    public partial class MyCatalog : IAsyncDisposable
    {
        // services

        [Inject] AuthenticationService AuthenticationService { get; set; } = default!;
        [Inject] ProtectedSessionStorage ProtectedSessionStorage { get; set; } = default!;
        [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] FileHandlingService FileHandlingService { get; set; } = default!;
        [Inject] ForgeServicesManager ForgeServicesManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private HubConnection? _hubConnection;
        private User? _loggedInUser;
        private Subscription? _subscription;
        private CatalogFolder? _selectedCatalogFolder;
        private List<CatalogFolder> _catalogFolders = new();
        private List<CatalogModel> _catalogModels = new();
        private bool _displayListView = false;
        private const string _displayListViewSettingKey = "EFAE9570-C7C5-45FC-BF26-86E69F8DB677";
        private bool _drawerOpen = true;
        private bool _initializing = true;
        private bool _uploadInProgress = false;
        private string _uploadProgressMessage = string.Empty;


        // new cad model dialog
        private DialogOptions _newCatalogModelDialogOptions = new() 
        { 
            FullWidth = true, 
            MaxWidth = MaxWidth.Medium, 
            DisableBackdropClick = true,
            CloseOnEscapeKey = false
        };
        private bool _showNewCatalogModelDialog = false;
        private string? _newCatalogModelDisplayName;
        private string? _newCatalogModelDescription;
        private bool _newCatalogModelValid = false;
        private string _dragStyle = string.Empty;
        private IBrowserFile? _attachedFile;
        private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
        private void ClearDragStyle() => _dragStyle = string.Empty;
        private IProgress<string> _progress = default!;

        protected override async Task OnInitializedAsync()
        {
            // get data
            _loggedInUser = await AuthenticationService.GetUser();

            if (_loggedInUser == null || _loggedInUser.SubscriptionId == null)
            {
                NavigationManager.NavigateTo("/notauthorized");
                return;
            }

            _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionById((int)_loggedInUser.SubscriptionId!);
            _catalogFolders = await DataServicesManager.CatalogService.GetCatalogFoldersBySubscriptionId(_subscription!.Id);

            // setup signal R hub connection for model derivative callback
            _hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/forgecallbackhub"))
                                                       .WithAutomaticReconnect()
                                                       .Build();

            _hubConnection.On<string, string>(nameof(ForgeCallbackController.ModelDerivativeTranslation_OnComplete), ModelDerivativeTranslation_OnComplete);

            _progress = new Progress<string>(message =>
            {
                _uploadInProgress = true;
                _uploadProgressMessage = message;
                StateHasChanged();
            });

            _initializing = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // load view setting
                var viewSetting = await ProtectedSessionStorage.GetAsync<bool>(_displayListViewSettingKey);
                _displayListView = viewSetting.Success ? viewSetting.Value : false;
                StateHasChanged();
            }
        }

        private async Task ToggleView_OnClick()
        {
            _displayListView = !_displayListView;
            await ProtectedSessionStorage.SetAsync(_displayListViewSettingKey, _displayListView);
        }



        #region "catalog folders"

        private async Task CatalogFolder_OnClick(CatalogFolder? selectedFolder)
        {
            _selectedCatalogFolder = selectedFolder;

            if (_selectedCatalogFolder == null)
            {
                _catalogModels.Clear();
                return;
            }

            _catalogModels = await DataServicesManager.CatalogService.GetCatalogModelsByCatalogFolderId(_selectedCatalogFolder.Id);
        }

        private async Task NewCatalogFolder_OnClick()
        {
            DialogResult result = await DialogService.Show<NewCatalogFolderDialog>("New Folder").Result;

            if (result.Canceled)
                return;

            string newCatalogFolderName = (string)result.Data;

            if (CatalogFolderNameIsDuplicate(_selectedCatalogFolder, newCatalogFolderName))
            {
                Snackbar.Add("Folder name already used!", Severity.Warning);
                return;
            }

            CatalogFolder newFolder = await DataServicesManager.CatalogService.CreateCatalogFolder(subscriptionId: _subscription!.Id,
                                                                                                   createdById: _loggedInUser!.Id,
                                                                                                   displayName: newCatalogFolderName,
                                                                                                   parentId: _selectedCatalogFolder?.Id);

            if (_selectedCatalogFolder == null)
            {
                _catalogFolders.Add(newFolder);
                _catalogFolders.Sort();
            }
            else
            {
                newFolder.ParentFolder = _selectedCatalogFolder;
                _selectedCatalogFolder.ChildFolders.Add(newFolder);
                _selectedCatalogFolder.ChildFolders.Sort();
            }
        }

        private async Task RenameCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            DialogParameters parameters = new()
            {
                { nameof(RenameDialog.NewName), catalogFolder.DisplayName },
                { nameof(RenameDialog.MaxLength), 50 }
            };

            DialogResult result = await DialogService.Show<RenameDialog>("Rename Folder", parameters).Result;

            if (result.Canceled)
                return;

            string newDisplayName = (string)result.Data;

            if (CatalogFolderNameIsDuplicate(catalogFolder.ParentFolder, newDisplayName))
            {
                Snackbar.Add("Folder name already used!", Severity.Warning);
                return;
            }

            catalogFolder.DisplayName = newDisplayName;
            await DataServicesManager.CatalogService.UpdateCatalogFolder(catalogFolder);

            Snackbar.Add("Folder renamed successfully!", Severity.Success);
        }

        private async Task MoveCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            DialogParameters parameters = new()
            {
                { nameof(MoveCatalogFolderDialog.CatalogFolders), _catalogFolders },
                { nameof(MoveCatalogFolderDialog.FolderToMove), catalogFolder },
            };

            DialogResult result = await DialogService.Show<MoveCatalogFolderDialog>($"Move \"{catalogFolder.DisplayName}\"", parameters).Result;

            if (result.Canceled)
                return;

            CatalogFolder newParentFolder = (CatalogFolder)result.Data;

            if (newParentFolder == catalogFolder.ParentFolder)
                return;

            if (CatalogFolderNameIsDuplicate(newParentFolder, catalogFolder.DisplayName))
            {
                Snackbar.Add("Folder name already used!", Severity.Warning);
                return;
            }

            // update database
            catalogFolder.ParentId = newParentFolder.Id;
            await DataServicesManager.CatalogService.UpdateCatalogFolder(catalogFolder);

            // refresh UI
            newParentFolder.ChildFolders.Add(catalogFolder);
            catalogFolder.ParentFolder?.ChildFolders.Remove(catalogFolder);
            catalogFolder.ParentFolder = newParentFolder;

            //// TO DO: figure out how to update the UI if the selected folder was moved
            //if(_selectedCatalogFolder == catalogFolder)
            //    await CatalogFolder_OnClick(catalogFolder.ParentFolder);

            Snackbar.Add("Folder moved successfully!", Severity.Success);
        }

        private async Task DeleteCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            // make sure folder is empty
            var catalogModels = await DataServicesManager.CatalogService.GetCatalogModelsByCatalogFolderId(catalogFolder.Id);

            if (catalogFolder.ChildFolders.Any() || catalogModels.Any())
            {
                Snackbar.Add("Folder must be empty!", Severity.Warning);
                return;
            }

            bool? confirmDelete = await DialogService.ShowMessageBox(title: "Delete Folder",
                                                                     message: "Are you sure you want to delete this folder?",
                                                                     yesText: "Yes",
                                                                     cancelText: "Cancel");
            if (confirmDelete != true)
                return;

            // delete record from database
            await DataServicesManager.CatalogService.DeleteCatalogFolderById(catalogFolder.Id);

            // refresh UI
            if(catalogFolder.ParentFolder == null)
            {
                _catalogFolders.Remove(catalogFolder);
            }
            else
            {
                catalogFolder.ParentFolder.ChildFolders.Remove(catalogFolder);
            }

            // move up one folder level 
            if(_selectedCatalogFolder == catalogFolder)
                await CatalogFolder_OnClick(catalogFolder.ParentFolder);

            Snackbar.Add("Folder deleted successfully!", Severity.Success);
        }

        private bool CatalogFolderNameIsDuplicate(CatalogFolder? parentFolder, string displayName)
        {
            if(parentFolder == null)
                return _catalogFolders.Any(i => i.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            return parentFolder.ChildFolders.Any(i => i.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion



        #region "catalog models"

        private void CatalogModelsGrid_OnRowClick(DataGridRowClickEventArgs<CatalogModel> args)
        {
            NavigationManager.NavigateTo($"catalog/{_subscription!.SubdirectoryName}/{args.Item.Guid}");
        }

        private async Task NewCatalogModelDialog_OnSubmit()
        {
            if (!_newCatalogModelValid)
                return;

            if (_attachedFile == null)
                return;

            if (_selectedCatalogFolder == null)
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
                _progress.Report("Uploading to Cadflair...");
                tempFilename = await FileHandlingService.UploadBrowserFileToTempFolder(_attachedFile);

                // upload file to Autodesk OSS
                _progress.Report("Uploading to Autodesk OSS...");
                string bucketKey = Guid.NewGuid().ToString();
                await ForgeServicesManager.ObjectStorageService.CreateBucket(bucketKey);

                string objectKey = _attachedFile.Name;
                bool uploadSuccessful = await ForgeServicesManager.ObjectStorageService.UploadFile(bucketKey, objectKey, tempFilename);

                if (!uploadSuccessful)
                    throw new Exception("Could not upload to Autodesk OSS!");

                // start the model translation
                _progress.Report("Translating model...");

                // connect to Signal R hub
                if (_hubConnection?.State != HubConnectionState.Connected)
                    await _hubConnection?.StartAsync()!;

                await ForgeServicesManager.ModelDerivativeService.TranslateObject(bucketKey, objectKey, false, null, _hubConnection.ConnectionId);
            }
            catch (Exception ex)
            {
                Snackbar.Add("An error occurred!", Severity.Error);
                Debug.WriteLine(ex);
            }

            // clean up
            if (tempFilename != null)
                File.Delete(tempFilename);
        }

        private void NewCatalogModelDialog_OnClose()
        {
            _uploadInProgress = false;
            _newCatalogModelDisplayName = null;
            _newCatalogModelDescription = null;
            _attachedFile = null;
            _showNewCatalogModelDialog = false;
        }

        private async Task ModelDerivativeTranslation_OnComplete(string bucketKey, string objectKey)
        {
            // create database record
            _progress.Report("Updating database...");
            CatalogModel catalogModel = await DataServicesManager.CatalogService.CreateCatalogModel(subscriptionId: _subscription!.Id,
                                                                                                    catalogFolderId: _selectedCatalogFolder!.Id,
                                                                                                    createdById: _loggedInUser!.Id,
                                                                                                    displayName: _newCatalogModelDisplayName,
                                                                                                    description: _newCatalogModelDescription,
                                                                                                    bucketKey: bucketKey,
                                                                                                    objectKey: objectKey);

            _catalogModels.Add(catalogModel);
            _catalogModels.Sort();

            Snackbar.Add("Model uploaded successfully!", Severity.Success);
            NewCatalogModelDialog_OnClose();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteCatalogModel_OnClick(CatalogModel catalogModel)
        {
            bool? confirmDelete = await DialogService.ShowMessageBox(title: "Delete Model",
                                                                     message: "Are you sure you want to delete this model?",
                                                                     yesText: "Yes",
                                                                     cancelText: "Cancel");
            if (confirmDelete != true)
                return;

            // delete file from Autodesk OSS
            await ForgeServicesManager.ObjectStorageService.DeleteBucket(catalogModel.BucketKey);

            // delete record from database
            await DataServicesManager.CatalogService.DeleteCatalogModelById(catalogModel.Id);

            _catalogModels.Remove(catalogModel);
            Snackbar.Add("Model deleted successfully!", Severity.Success);
        }

        #endregion




        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
                await _hubConnection.DisposeAsync();
        }

    }
}
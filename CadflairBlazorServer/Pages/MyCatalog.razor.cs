using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Pages
{
    public partial class MyCatalog
    {
        // services
        [Inject] AuthenticationService AuthenticationService { get; set; } = default!;
        [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] FileHandlingService FileHandlingService { get; set; } = default!;
        [Inject] ForgeServicesManager ForgeServicesManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] ILogger<MyCatalog> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private User? _loggedInUser;
        private Subscription? _subscription;
        private CatalogFolder? _selectedCatalogFolder;
        private List<CatalogFolder> _catalogFolders = new();
        private List<CatalogModel> _catalogModels = new();
        private bool _drawerOpen = true;
        private bool _initializing = true;
        private bool _loadingCatalogModels = false;
        private bool _uploadInProgress = false;

        // new cad model dialog
        private bool _showUploadDialog = false;
        private DialogOptions _uploadDialogOptions = new() 
        { 
            FullWidth = true, 
            MaxWidth = MaxWidth.Medium, 
            DisableBackdropClick = true,
            CloseOnEscapeKey = false
        };

        private class CadModelUpload
        {
            public IBrowserFile File { get; set; } = default!;
            public string Status { get; set; } = string.Empty;
        }

        private List<CadModelUpload> _uploadList = new();

        private string _dragStyle = string.Empty;
        private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
        private void ClearDragStyle() => _dragStyle = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
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
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }


        #region "catalog folders"

        private async Task LoadCatalogModels(CatalogFolder? catalogFolder)
        {
            try
            {
                _loadingCatalogModels = true;
                _catalogModels.Clear();

                if (catalogFolder == null)
                    return;

                _catalogModels = await DataServicesManager.CatalogService.GetCatalogModelsByCatalogFolderId(catalogFolder.Id);
                _loadingCatalogModels = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Faild to load CatalogModels - Id: {catalogFolder?.Id}");
                Snackbar.Add("An error occurred!", Severity.Error);
            }

        }

        private async Task CatalogFolder_OnClick(CatalogFolder? selectedFolder)
        {
            _selectedCatalogFolder = selectedFolder;
            await LoadCatalogModels(selectedFolder);
        }

        private async Task NewCatalogFolder_OnClick()
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while creating new CatalogFolder!");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private async Task RenameCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while renaming CatalogFolder - Id: {catalogFolder.Id}");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private async Task MoveCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            // TO DO: I may want to find a way to move a folder to the root level (set parent folder to NULL).

            try
            {
                DialogParameters parameters = new()
                {
                    { nameof(SelectCatalogFolderDialog.CatalogFolders), _catalogFolders },
                    { nameof(SelectCatalogFolderDialog.CurrentLocation), catalogFolder.ParentFolder },
                    { nameof(SelectCatalogFolderDialog.FolderToHide), catalogFolder },
                };

                DialogResult result = await DialogService.Show<SelectCatalogFolderDialog>($"Move \"{catalogFolder.DisplayName}\"", parameters).Result;

                if (result.Canceled)
                    return;

                CatalogFolder newParentFolder = (CatalogFolder)result.Data;

                if (newParentFolder.Id == catalogFolder.ParentFolder?.Id)
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
                if(catalogFolder.ParentFolder == null)
                {
                    _catalogFolders.Remove(catalogFolder);
                }
                else
                {
                    catalogFolder.ParentFolder.ChildFolders.Remove(catalogFolder);
                }
                catalogFolder.ParentFolder = newParentFolder;
                newParentFolder.ChildFolders.Add(catalogFolder);

                Snackbar.Add("Folder moved successfully!", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while moving CatalogFolder - Id: {catalogFolder.Id}");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private async Task DeleteCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            try
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
                if (catalogFolder.ParentFolder == null)
                {
                    _catalogFolders.Remove(catalogFolder);
                }
                else
                {
                    catalogFolder.ParentFolder.ChildFolders.Remove(catalogFolder);
                }

                // move up one folder level if selected folder was deleted
                if (_selectedCatalogFolder == catalogFolder)
                {
                    _selectedCatalogFolder = catalogFolder.ParentFolder;
                    await LoadCatalogModels(catalogFolder.ParentFolder);
                }

                Snackbar.Add("Folder deleted successfully!", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while deleting CatalogFolder - Id: {catalogFolder.Id}");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private bool CatalogFolderNameIsDuplicate(CatalogFolder? parentFolder, string displayName)
        {
            if (parentFolder == null)
                return _catalogFolders.Any(i => i.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            return parentFolder.ChildFolders.Any(i => i.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion



        #region "catalog models"

        private async Task CatalogModelUpload_FilesChanged(IReadOnlyList<IBrowserFile> files)
        {
            _uploadInProgress = true;

            _uploadList.Clear();
            _uploadList = files.Select(file => new CadModelUpload
            {
                File = file,
                Status = "Not started",
            }).ToList();

            foreach (CadModelUpload upload in _uploadList)
            {
                string? tempFilename = null;

                try
                {
                    string[] validExtensions = { ".ipt", ".stp" }; 

                    if (!validExtensions.Any(i => i == Path.GetExtension(upload.File.Name)))
                    {
                        //Snackbar.Add("Invalid file extension!", Severity.Warning);
                        upload.Status = "Invalid file extension";
                        StateHasChanged();
                        continue;
                    }

                    // check for duplicate model name in the selected folder
                    if (_catalogModels.Any(i => i.DisplayName == upload.File.Name))
                    {
                        upload.Status = "Invalid file name";
                        StateHasChanged();
                        continue;
                    }

                    upload.Status = "Processing";
                    StateHasChanged();

                    // save file to server
                    tempFilename = await FileHandlingService.UploadBrowserFileToTempFolder(upload.File);

                    // upload file to Autodesk OSS
                    string bucketKey = Guid.NewGuid().ToString();
                    await ForgeServicesManager.ObjectStorageService.CreateBucket(bucketKey);

                    string objectKey = upload.File.Name;
                    bool uploadSuccessful = await ForgeServicesManager.ObjectStorageService.UploadFile(bucketKey, objectKey, tempFilename);

                    if (!uploadSuccessful)
                        throw new Exception("Could not upload to Autodesk OSS!");

                    // start the model translation
                    await ForgeServicesManager.ModelDerivativeService.TranslateObject(bucketKey, objectKey, false, null);

                    // create database record
                    CatalogModel catalogModel = await DataServicesManager.CatalogService.CreateCatalogModel(subscriptionId: _subscription!.Id,
                                                                                                            catalogFolderId: _selectedCatalogFolder!.Id,
                                                                                                            createdById: _loggedInUser!.Id,
                                                                                                            displayName: objectKey,
                                                                                                            description: null,
                                                                                                            bucketKey: bucketKey,
                                                                                                            objectKey: objectKey);
                    upload.Status = "Success";
                    StateHasChanged();

                    _catalogModels.Add(catalogModel);
                }
                catch (Exception ex)
                {
                    upload.Status = "Error";
                    StateHasChanged();

                    Logger.LogError(ex, $"Error occurred while creating CatalogModel!");
                }

                // clean up
                if (tempFilename != null)
                    File.Delete(tempFilename);
            }

            _uploadInProgress = false;
        }

        private void UploadDialog_OnClose()
        {
            _uploadList.Clear();
            _showUploadDialog = false;
        }

        private async Task MoveCatalogModel_OnClick(CatalogModel catalogModel)
        {
            try
            {
                DialogParameters parameters = new()
                {
                    { nameof(SelectCatalogFolderDialog.CatalogFolders), _catalogFolders },
                    { nameof(SelectCatalogFolderDialog.CurrentLocation), _selectedCatalogFolder },
                };

                DialogResult result = await DialogService.Show<SelectCatalogFolderDialog>($"Move \"{catalogModel.DisplayName}\"", parameters).Result;

                if (result.Canceled)
                    return;

                CatalogFolder selectedFolder = (CatalogFolder)result.Data;

                if (selectedFolder.Id == catalogModel.CatalogFolderId)
                    return;

                // check for duplicate model name in the selected folder
                var existingModels = await DataServicesManager.CatalogService.GetCatalogModelsByCatalogFolderId(selectedFolder.Id);
                if (existingModels.Any(i => i.DisplayName == catalogModel.DisplayName))
                {
                    Snackbar.Add("Model name already used!", Severity.Warning);
                    return;
                }

                // update database
                catalogModel.CatalogFolderId = selectedFolder.Id;
                await DataServicesManager.CatalogService.UpdateCatalogModel(catalogModel);

                // refresh UI
                _catalogModels.Remove(catalogModel);

                Snackbar.Add("Model moved successfully!", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while moving CatalogModel - Id: {catalogModel.Id}");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        private async Task DeleteCatalogModel_OnClick(CatalogModel catalogModel)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while deleting catalog model - CatalogModel Id: {catalogModel.Id}");
                Snackbar.Add("An error occurred!", Severity.Error);
            }
        }

        //private async Task RegenerateCatalogModelDerivative_OnClick(CatalogModel catalogModel)
        //{
        //    try
        //    {
        //        dynamic forgeObject = await ForgeServicesManager.ObjectStorageService.GetObjectDetails(catalogModel.BucketKey, catalogModel.ObjectKey);

        //        if (await ForgeServicesManager.ModelDerivativeService.TranslationExists(forgeObject.encoded_urn))
        //            await ForgeServicesManager.ModelDerivativeService.DeleteTranslation(forgeObject.encoded_urn);

        //        await ForgeServicesManager.ModelDerivativeService.TranslateObject(catalogModel.BucketKey, catalogModel.BucketKey, catalogModel.IsZip, catalogModel.RootFileName, _hubConnection.ConnectionId);
        //        Snackbar.Add("Generating new preview...", Severity.Success);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, $"Error occurred while regenerating new model derivative - CatalogModel Id: {catalogModel.Id}");
        //        Snackbar.Add("An error occurred!", Severity.Error);
        //    }
        //}

        #endregion

    }
}
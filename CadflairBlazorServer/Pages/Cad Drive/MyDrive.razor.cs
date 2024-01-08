using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class MyDrive
{
    // services
    [Inject] AuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] FileHandlingService FileHandlingService { get; set; } = default!;
    [Inject] ForgeServicesManager ForgeServicesManager { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;
    [Inject] ILogger<MyDrive> Logger { get; set; } = default!;

    // parameters
    [Parameter] public string CompanyName { get; set; } = string.Empty;

    // fields
    private User? _loggedInUser;
    private Subscription? _subscription;
    private DriveFolder? _selectedFolder;
    private List<DriveFolder> _folders = new();
    private List<DriveDocument> _documents = new();
    private bool _drawerOpen = true;
    private bool _initializing = true;
    private bool _loadingDocuments = false;
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

    private class DocumentUpload
    {
        public IBrowserFile File { get; set; } = default!;
        public string Status { get; set; } = string.Empty;
    }

    private List<DocumentUpload> _uploadList = new();

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
            _folders = await DataServicesManager.CadDriveService.GetDriveFoldersBySubscriptionId(_subscription!.Id);
            _initializing = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while initializing MyCatalog page!");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }


    #region "folders"

    private async Task LoadDriveDocuments(DriveFolder? folder)
    {
        try
        {
            _loadingDocuments = true;
            _documents.Clear();

            if (folder == null)
                return;

            _documents = await DataServicesManager.CadDriveService.GetDriveDocumentsByDriveFolderId(folder.Id);
            _loadingDocuments = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Faild to load models - DriveFolder.Id: {folder?.Id}");
            Snackbar.Add("An error occurred!", Severity.Error);
        }

    }

    private async Task Folder_OnClick(DriveFolder? selectedFolder)
    {
        _selectedFolder = selectedFolder;
        await LoadDriveDocuments(selectedFolder);
    }

    private async Task NewFolder_OnClick()
    {
        try
        {
            DialogResult result = await DialogService.Show<NewDriveFolderDialog>("New Folder").Result;

            if (result.Canceled)
                return;

            string newFolderName = (string)result.Data;

            if (FolderNameIsDuplicate(_selectedFolder, newFolderName))
            {
                Snackbar.Add("Folder name already used!", Severity.Warning);
                return;
            }

            DriveFolder newFolder = await DataServicesManager.CadDriveService.CreateDriveFolder(subscriptionId: _subscription!.Id,
                                                                                                   createdById: _loggedInUser!.Id,
                                                                                                   displayName: newFolderName,
                                                                                                   parentId: _selectedFolder?.Id);

            if (_selectedFolder == null)
            {
                _folders.Add(newFolder);
                _folders.Sort();
            }
            else
            {
                newFolder.ParentFolder = _selectedFolder;
                _selectedFolder.ChildFolders.Add(newFolder);
                _selectedFolder.ChildFolders.Sort();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while creating new DriveFolder!");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }

    private async Task RenameFolder_OnClick(DriveFolder folder)
    {
        try
        {
            DialogParameters parameters = new()
            {
                { nameof(RenameDialog.NewName), folder.DisplayName },
                { nameof(RenameDialog.MaxLength), 50 }
            };

            DialogResult result = await DialogService.Show<RenameDialog>("Rename Folder", parameters).Result;

            if (result.Canceled)
                return;

            string newDisplayName = (string)result.Data;

            if (FolderNameIsDuplicate(folder.ParentFolder, newDisplayName))
            {
                Snackbar.Add("Folder name already used!", Severity.Warning);
                return;
            }

            folder.DisplayName = newDisplayName;
            await DataServicesManager.CadDriveService.UpdateDriveFolder(folder);

            Snackbar.Add("Folder renamed successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while renaming CatalogFolder - Id: {folder.Id}");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }

    private async Task MoveFolder_OnClick(DriveFolder folder)
    {
        // TO DO: I may want to find a way to move a folder to the root level (set parent folder to NULL).

        try
        {
            DialogParameters parameters = new()
            {
                { nameof(SelectDriveFolderDialog.DriveFolders), _folders },
                { nameof(SelectDriveFolderDialog.CurrentLocation), folder.ParentFolder },
                { nameof(SelectDriveFolderDialog.FolderToHide), folder },
            };

            DialogResult result = await DialogService.Show<SelectDriveFolderDialog>($"Move \"{folder.DisplayName}\"", parameters).Result;

            if (result.Canceled)
                return;

            DriveFolder newParentFolder = (DriveFolder)result.Data;

            if (newParentFolder.Id == folder.ParentFolder?.Id)
                return;

            if (FolderNameIsDuplicate(newParentFolder, folder.DisplayName))
            {
                Snackbar.Add("Folder name already used!", Severity.Warning);
                return;
            }

            // update database
            folder.ParentId = newParentFolder.Id;
            await DataServicesManager.CadDriveService.UpdateDriveFolder(folder);

            // refresh UI
            if(folder.ParentFolder == null)
            {
                _folders.Remove(folder);
            }
            else
            {
                folder.ParentFolder.ChildFolders.Remove(folder);
            }
            folder.ParentFolder = newParentFolder;
            newParentFolder.ChildFolders.Add(folder);

            Snackbar.Add("Folder moved successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while moving CatalogFolder - Id: {folder.Id}");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }

    private async Task DeleteFolder_OnClick(DriveFolder folder)
    {
        try
        {
            // make sure folder is empty
            var documents = await DataServicesManager.CadDriveService.GetDriveDocumentsByDriveFolderId(folder.Id);

            if (folder.ChildFolders.Any() || documents.Any())
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
            await DataServicesManager.CadDriveService.DeleteDriveFolderById(folder.Id);

            // refresh UI
            if (folder.ParentFolder == null)
            {
                _folders.Remove(folder);
            }
            else
            {
                folder.ParentFolder.ChildFolders.Remove(folder);
            }

            // move up one folder level if selected folder was deleted
            if (_selectedFolder == folder)
            {
                _selectedFolder = folder.ParentFolder;
                await LoadDriveDocuments(folder.ParentFolder);
            }

            Snackbar.Add("Folder deleted successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while deleting DriveFolder - Id: {folder.Id}");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }

    private bool FolderNameIsDuplicate(DriveFolder? parentFolder, string displayName)
    {
        if (parentFolder == null)
            return _folders.Any(i => i.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));

        return parentFolder.ChildFolders.Any(i => i.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
    }

    #endregion



    #region "documents"

    private async Task DocumentUpload_FilesChanged(IReadOnlyList<IBrowserFile> files)
    {
        _uploadInProgress = true;

        _uploadList.Clear();
        _uploadList = files.Select(file => new DocumentUpload
        {
            File = file,
            Status = "Not started",
        }).ToList();

        foreach (DocumentUpload upload in _uploadList)
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
                if (_documents.Any(i => i.DisplayName == upload.File.Name))
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
                DriveDocument catalogModel = await DataServicesManager.CadDriveService.CreateDriveDocument(subscriptionId: _subscription!.Id,
                                                                                                        folderId: _selectedFolder!.Id,
                                                                                                        createdById: _loggedInUser!.Id,
                                                                                                        displayName: objectKey,
                                                                                                        description: null,
                                                                                                        bucketKey: bucketKey,
                                                                                                        objectKey: objectKey);
                upload.Status = "Success";
                StateHasChanged();

                _documents.Add(catalogModel);
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

    private async Task MoveDocument_OnClick(DriveDocument document)
    {
        try
        {
            DialogParameters parameters = new()
            {
                { nameof(SelectDriveFolderDialog.DriveFolders), _folders },
                { nameof(SelectDriveFolderDialog.CurrentLocation), _selectedFolder },
            };

            DialogResult result = await DialogService.Show<SelectDriveFolderDialog>($"Move \"{document.DisplayName}\"", parameters).Result;

            if (result.Canceled)
                return;

            DriveFolder selectedFolder = (DriveFolder)result.Data;

            if (selectedFolder.Id == document.DriveFolderId)
                return;

            // check for duplicate model name in the selected folder
            var existingModels = await DataServicesManager.CadDriveService.GetDriveDocumentsByDriveFolderId(selectedFolder.Id);
            if (existingModels.Any(i => i.DisplayName == document.DisplayName))
            {
                Snackbar.Add("Model name already used!", Severity.Warning);
                return;
            }

            // update database
            document.DriveFolderId = selectedFolder.Id;
            await DataServicesManager.CadDriveService.UpdateDriveDocument(document);

            // refresh UI
            _documents.Remove(document);

            Snackbar.Add("Model moved successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while moving DriveDocument - Id: {document.Id}");
            Snackbar.Add("An error occurred!", Severity.Error);
        }
    }

    private async Task DeleteDocument_OnClick(DriveDocument document)
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
            await ForgeServicesManager.ObjectStorageService.DeleteBucket(document.BucketKey);

            // delete record from database
            await DataServicesManager.CadDriveService.DeleteDriveDocumentById(document.Id);

            _documents.Remove(document);
            Snackbar.Add("Model deleted successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while deleting document - DriveDocument Id: {document.Id}");
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
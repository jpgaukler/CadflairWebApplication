using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class DriveFolderTree
{
    // parameters
    [Parameter] public List<DriveFolder> DriveFolders { get; set; } = new();
    [Parameter] public DriveFolder? SelectedDriveFolder { get; set; }
    [Parameter] public bool ShowMoreMenu { get; set; } = false;
    [Parameter] public EventCallback<DriveFolder?> SelectedDriveFolderChanged { get; set; }
    [Parameter] public EventCallback<DriveFolder> MoveDriveFolderClicked { get; set; }
    [Parameter] public EventCallback<DriveFolder> RenameDriveFolderClicked { get; set; }
    [Parameter] public EventCallback<DriveFolder> DeleteDriveFolderClicked { get; set; }

    /// <summary>
    /// Hide a specific folder so that it does not show up in the tree. (This is used to prevent circular parent folder references when moving a folder).
    /// </summary>
    [Parameter] public DriveFolder? FolderToHide { get; set; }

    // fields
    private Func<DriveFolder, bool> _filter => folder =>
    {
        if (FolderToHide == null)
            return true;

        if (folder.Id == FolderToHide.Id)
            return false;

        return true;
    };

    private async Task CatalogFolder_OnClick(DriveFolder selectedFolder)
    {
        if (SelectedDriveFolder == selectedFolder)
        {
            SelectedDriveFolder = null;
        }
        else
        {
            SelectedDriveFolder = selectedFolder;
        }

        await SelectedDriveFolderChanged.InvokeAsync(SelectedDriveFolder);
    }

    private async Task RenameCatalogFolder_OnClick(DriveFolder catalogFolder)
    {
        await RenameDriveFolderClicked.InvokeAsync(catalogFolder);
    }

    private async Task MoveCatalogFolder_OnClick(DriveFolder catalogFolder)
    {
        await MoveDriveFolderClicked.InvokeAsync(catalogFolder);
    }

    private async Task DeleteCatalogFolder_OnClick(DriveFolder catalogFolder)
    {
        await DeleteDriveFolderClicked.InvokeAsync(catalogFolder);
    }
}
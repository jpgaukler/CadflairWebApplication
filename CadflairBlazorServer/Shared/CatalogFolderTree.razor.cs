using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class CatalogFolderTree
    {
        // parameters
        [Parameter] public List<CatalogFolder> CatalogFolders { get; set; } = new();
        [Parameter] public CatalogFolder? SelectedCatalogFolder { get; set; }
        [Parameter] public bool ShowMoreMenu { get; set; } = false;
        [Parameter] public EventCallback<CatalogFolder?> SelectedCatalogFolderChanged { get; set; }
        [Parameter] public EventCallback<CatalogFolder> MoveCatalogFolderClicked { get; set; }
        [Parameter] public EventCallback<CatalogFolder> RenameCatalogFolderClicked { get; set; }
        [Parameter] public EventCallback<CatalogFolder> DeleteCatalogFolderClicked { get; set; }

        /// <summary>
        /// Hide a specific folder so that it does not show up in the tree. (This is used to prevent circular parent folder references when moving a folder).
        /// </summary>
        [Parameter] public CatalogFolder? FolderToHide { get; set; }

        // fields
        private Func<CatalogFolder, bool> _filter => folder =>
        {
            if (FolderToHide == null)
                return true;

            if (folder.Id == FolderToHide.Id)
                return false;

            return true;
        };

        private async Task CatalogFolder_OnClick(CatalogFolder selectedFolder)
        {
            if (SelectedCatalogFolder == selectedFolder)
            {
                SelectedCatalogFolder = null;
            }
            else
            {
                SelectedCatalogFolder = selectedFolder;
            }

            await SelectedCatalogFolderChanged.InvokeAsync(SelectedCatalogFolder);
        }

        private async Task RenameCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            await RenameCatalogFolderClicked.InvokeAsync(catalogFolder);
        }

        private async Task MoveCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            await MoveCatalogFolderClicked.InvokeAsync(catalogFolder);
        }

        private async Task DeleteCatalogFolder_OnClick(CatalogFolder catalogFolder)
        {
            await DeleteCatalogFolderClicked.InvokeAsync(catalogFolder);
        }
    }
}
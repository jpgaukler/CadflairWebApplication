using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class CatalogFolderTree
    {
        // parameters
        [Parameter] public List<CatalogFolder> CatalogFolders { get; set; } = new();
        [Parameter] public CatalogFolder? SelectedCatalogFolder { get; set; }
        [Parameter] public EventCallback<CatalogFolder?> SelectedCatalogFolderChanged { get; set; }

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
    }
}
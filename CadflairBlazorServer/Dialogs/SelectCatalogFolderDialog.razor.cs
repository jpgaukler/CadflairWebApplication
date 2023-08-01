using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Dialogs
{
    public partial class SelectCatalogFolderDialog
    {
        // parameters
        [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
        [Parameter] public List<CatalogFolder> CatalogFolders { get; set; } = new();
        [Parameter] public CatalogFolder CurrentLocation { get; set; } = default!;
        [Parameter] public CatalogFolder? FolderToHide { get; set; } = default!;

        // fields
        private CatalogFolder? _selectedCatalogFolder;

        protected override void OnInitialized()
        {
            var options = new DialogOptions 
            { 
                CloseButton = false,
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseOnEscapeKey = false 
            };

            MudDialogInstance.SetOptions(options);
        }

        private void Cancel_OnClick() => MudDialogInstance.Cancel();
        private void Ok_OnClick() => MudDialogInstance.Close(_selectedCatalogFolder);
    }
}
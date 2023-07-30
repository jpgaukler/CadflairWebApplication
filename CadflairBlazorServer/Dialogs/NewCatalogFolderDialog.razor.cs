using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Dialogs
{
    public partial class NewCatalogFolderDialog
    {
        // parameters
        [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;

        // fields
        private string? _newCatalogFolderName;

        protected override void OnInitialized()
        {
            var options = new DialogOptions 
            { 
                CloseButton = false,
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true,
                CloseOnEscapeKey = false 
            };

            MudDialogInstance.SetOptions(options);
        }

        private void Cancel_OnClick() => MudDialogInstance.Cancel();
        private void Ok_OnClick() => MudDialogInstance.Close(_newCatalogFolderName);
    }
}
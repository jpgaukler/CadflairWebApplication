using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class RenameDialog
    {
        // parameters
        [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
        [Parameter] public int MaxLength { get; set; }

        // fields
        private string? _newName { get; set; }

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
        private void Ok_OnClick() => MudDialogInstance.Close(_newName);
    }
}
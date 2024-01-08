using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class NewDriveFolderDialog
{
    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;

    // fields
    private string? _newFolderName;

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
    private void Ok_OnClick() => MudDialogInstance.Close(_newFolderName);
}
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class SelectDriveFolderDialog
{
    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
    [Parameter] public List<DriveFolder> DriveFolders { get; set; } = new();
    [Parameter] public DriveFolder CurrentLocation { get; set; } = default!;
    [Parameter] public DriveFolder? FolderToHide { get; set; } = default!;

    // fields
    private DriveFolder? _selectedDriveFolder;

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
    private void Ok_OnClick() => MudDialogInstance.Close(_selectedDriveFolder);
}
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class AddColumnDialog
{
    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
    [Parameter] public string? Header { get; set; }

    // fields 
    private MudTextField<string?> _mudTextField = default!;


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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await _mudTextField.SelectAsync();
    }

    private void Cancel_OnClick() => MudDialogInstance.Cancel();
    private void Ok_OnClick() => MudDialogInstance.Close(this);
}
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class ViewerMobileDialog
{
    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
    [Parameter] public string BucketKey { get; set; } = string.Empty;
    [Parameter] public string ObjectKey { get; set; } = string.Empty;

    // fields
    private ForgeViewer? _forgeViewer;

    protected override void OnInitialized()
    {
        var options = new DialogOptions 
        { 
            NoHeader = true,
            CloseButton = true,
            FullScreen = true,
        };

        MudDialogInstance.SetOptions(options);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await _forgeViewer!.ViewDocument(BucketKey, ObjectKey);
    }

    private void Close_OnClick() => MudDialogInstance.Close();

}
using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class PreviewDialog
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
                CloseButton = true,
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                CloseOnEscapeKey = false 
            };

            MudDialogInstance.SetOptions(options);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await _forgeViewer!.ViewDocument(BucketKey, ObjectKey);
        }

        //private void Cancel_OnClick() => MudDialogInstance.Cancel();
        //private void Ok_OnClick() => MudDialogInstance.Close();
    }
}
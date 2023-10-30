using Microsoft.AspNetCore.Components;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class PreviewDialog
    {
        // parameters
        [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
        [Parameter] public string BucketKey { get; set; } = string.Empty;
        [Parameter] public string ObjectKey { get; set; } = string.Empty;
        [Parameter] public Row Row { get; set; } = default!;

        // fields
        private ForgeViewer? _forgeViewer;
        private Attachment? _selectedAttachment;

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

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //        await _forgeViewer!.ViewDocument(BucketKey, ObjectKey);
        //}

        private async Task PreviewAttachment_OnClick()
        {
            if (_selectedAttachment == null)
                return;

            await _forgeViewer!.ViewDocument(BucketKey, _selectedAttachment.ForgeObjectKey);
        }

        //private void Cancel_OnClick() => MudDialogInstance.Cancel();
        //private void Ok_OnClick() => MudDialogInstance.Close();
    }
}
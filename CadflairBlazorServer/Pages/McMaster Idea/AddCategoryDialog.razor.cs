using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class AddCategoryDialog
    {
        // parameters
        [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Description { get; set; }


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
        private void Ok_OnClick() => MudDialogInstance.Close(this);
    }
}
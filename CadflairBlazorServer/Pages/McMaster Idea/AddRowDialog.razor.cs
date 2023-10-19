using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class AddRowDialog
    {
        // parameters
        [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
        [Parameter] public string? Header { get; set; }
        [Parameter] public List<Column> Columns { get; set; } = default!;
        [Parameter] public List<TableValue> NewRowValues { get; set; } = new(); 


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
            NewRowValues = Columns.Select(i => new TableValue() { ColumnId = i.Id }).ToList();
        }

        private void Cancel_OnClick() => MudDialogInstance.Cancel();
        private void Ok_OnClick() => MudDialogInstance.Close(this);
    }
}
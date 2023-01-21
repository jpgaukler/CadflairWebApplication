using Microsoft.AspNetCore.Components;
using MudBlazor;
using CadflairDataAccess;
using CadflairDataAccess.Models;

namespace CadflairBlazorServer.Shared.Dialogs
{
    public partial class ProductFolderDialog
    {

        // parameters
        [CascadingParameter] public MudDialogInstance? MudDialog { get; set; }

        // fields
        private string _newProductFolderName = string.Empty;

        private void Ok_OnClick() => MudDialog?.Close(DialogResult.Ok(_newProductFolderName));

        private void Cancel_OnClick() => MudDialog?.Cancel();
    }
}
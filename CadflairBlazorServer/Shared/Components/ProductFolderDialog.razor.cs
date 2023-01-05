using Microsoft.AspNetCore.Components;
using MudBlazor;
using CadflairDataAccess;
using CadflairDataAccess.Models;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ProductFolderDialog
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;

        // parameters
        [CascadingParameter] public MudDialogInstance? MudDialog { get; set; }
        [Parameter] public int UserId { get; set; }
        [Parameter] public int SubscriptionId { get; set; }
        [Parameter] public int? ParentId { get; set; }

        // fields
        private string _newProductFolderName = string.Empty;

        private async Task Ok_OnClick()
        {
            List<ProductFolder> existingFolders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionIdAndParentId(SubscriptionId, ParentId);
            if (existingFolders.Exists(i => i.DisplayName.Equals(_newProductFolderName, StringComparison.CurrentCultureIgnoreCase)))
            {
                _snackbar.Add("A folder with this name already exists.", Severity.Warning);
                return;
            }

            ProductFolder productFolder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId: SubscriptionId, createdById: UserId, displayName: _newProductFolderName, parentId: ParentId);
            MudDialog?.Close(DialogResult.Ok(productFolder));
        }

        private void Cancel_OnClick() => MudDialog?.Cancel();
    }
}
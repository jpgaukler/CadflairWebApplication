using Microsoft.AspNetCore.Components;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class ProductDetails
    {
        // services
        [Inject] DataServicesManager  DataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] ILogger<Categories> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;
        [Parameter] public string ProductDefinitionName { get; set; } = string.Empty;
        [Parameter] public string PartNumber { get; set; } = string.Empty;

        // fields
        private Subscription? _subscription;
        private ProductDefinition? _productDefinition;
        private ProductTable _productTable = new();
        private Row? _row;
        private ForgeViewer? _forgeViewer;
        private Attachment? _selectedAttachment;
        private bool _initializing = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
                _productDefinition = await DataServicesManager.McMasterService.GetProductDefinitionByNameAndSubscriptionId(ProductDefinitionName, _subscription.Id);
                _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_productDefinition.Id);
                _row = _productTable.Rows.First(i => i.PartNumber == PartNumber);
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing products page!");
                NavigationManager.NavigateTo("/notfound");
                return;
            }
        }

        private async Task PreviewAttachment_OnClick()
        {
            if (_selectedAttachment == null)
                return;

            await _forgeViewer!.ViewDocument(_productDefinition!.ForgeBucketKey, _selectedAttachment.ForgeObjectKey);
        }

        private async Task Download_OnClick(string bucketKey, string objectKey)
        {
            string url = await ForgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(bucketKey, objectKey);
            NavigationManager.NavigateTo(url);
        }

    }
}
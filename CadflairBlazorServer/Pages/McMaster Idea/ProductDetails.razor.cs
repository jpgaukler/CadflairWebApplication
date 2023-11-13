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
        private Row _row = new();
        private ForgeViewer? _forgeViewer;
        private Attachment? _2dAttachment;
        private Attachment? _3dAttachment;
        private Attachment? _activeAttachment;
        private Attachment? _selectedDownload;
        private bool _disable2dButton = true;
        private bool _disable3dButton = true;
        private bool _initializing = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
                    _productDefinition = await DataServicesManager.McMasterService.GetProductDefinitionByNameAndSubscriptionId(ProductDefinitionName, _subscription.Id);
                    _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_productDefinition.Id);
                    _row = _productTable.Rows.First(i => i.PartNumber == PartNumber);
                    _selectedDownload = _row.Attachments.FirstOrDefault();
                    _2dAttachment = _row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".pdf"));
                    _3dAttachment = _row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".stp"));

                    if(_3dAttachment != null)
                    {
                        await Preview3D_OnClick();
                    }
                    else if (_2dAttachment != null)
                    {
                        await Preview2D_OnClick();
                    }

                    _initializing = false;
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error occurred while initializing products page!");
                    NavigationManager.NavigateTo("/notfound");
                }
            }
        }


        private async Task Preview2D_OnClick()
        {
            if (_2dAttachment == null)
                return;

            _activeAttachment = _2dAttachment;
            await _forgeViewer!.ViewDocument(_productDefinition!.ForgeBucketKey, _2dAttachment.ForgeObjectKey);
        }

        private async Task Preview3D_OnClick()
        {
            if (_3dAttachment == null)
                return;

            _activeAttachment = _3dAttachment;
            await _forgeViewer!.ViewDocument(_productDefinition!.ForgeBucketKey, _3dAttachment.ForgeObjectKey);
        }


        private void ShareButton_OnClick()
        {
            DialogParameters parameters = new()
            {
                { nameof(ShareDialog.Link) , NavigationManager.Uri }, 
            };

            DialogService.Show<ShareDialog>("Share", parameters);
        }


        private async Task Download_OnClick()
        {
            if (_selectedDownload == null)
                return;

            string url = await ForgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_productDefinition!.ForgeBucketKey, _selectedDownload.ForgeObjectKey);
            NavigationManager.NavigateTo(url);
        }

    }
}
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
        private Attachment? _selectedAttachment;
        private bool _initializing = true;
        private string _selectedOption = "3D";

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
                    await Attachment_ValueChanged(_row.Attachments.FirstOrDefault());
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

        //private async Task PreviewAttachment_OnClick()
        //{
        //    if (_selectedAttachment == null)
        //        return;

        //    await _forgeViewer!.ViewDocument(_productDefinition!.ForgeBucketKey, _selectedAttachment.ForgeObjectKey);
        //}

        private async Task Attachment_ValueChanged(Attachment? attachment)
        {
            _selectedAttachment = attachment;

            if (_selectedAttachment == null)
                return;

            await _forgeViewer!.ViewDocument(_productDefinition!.ForgeBucketKey, _selectedAttachment.ForgeObjectKey);
        }

        private async Task Preview2D_OnClick()
        {
            await Attachment_ValueChanged(_row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".pdf")));
        }

        private async Task Preview3D_OnClick()
        {
            await Attachment_ValueChanged(_row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".stp")));
        }

        private async Task SelectedOption_OnChange(string option)
        {
            _selectedOption = option;

            Attachment? attachment = null;

            if (option == "3D")
                attachment = _row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".stp"));

            if (option == "2D")
                attachment = _row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".pdf"));

            if (attachment == null)
                return;

            await _forgeViewer!.ViewDocument(_productDefinition!.ForgeBucketKey, attachment.ForgeObjectKey);
        }


        private async Task Download_OnClick()
        {
            if (_selectedAttachment == null)
                return;

            string url = await ForgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_productDefinition!.ForgeBucketKey, _selectedAttachment.ForgeObjectKey);
            NavigationManager.NavigateTo(url);
        }

    }
}
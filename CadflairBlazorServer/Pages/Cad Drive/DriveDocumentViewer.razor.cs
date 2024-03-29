using Microsoft.AspNetCore.Components;
using QRCoder;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class DriveDocumentViewer 
{
    // services
    [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
    [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] EmailService EmailService { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    // parameters
    [Parameter] public string Guid { get; set; } = string.Empty;
    [Parameter] public string SubdirectoryName { get; set; } = string.Empty;

    // fields
    private ForgeViewer? _forgeViewer;
    private Subscription? _subscription;
    private DriveDocument? _document;
    private bool _showActionButtons = true;
    private bool _minimizeActionButtons = false;
    private bool _initializing = true;

    // share dialog 
    private DialogOptions _shareDialogOptions = new() { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };
    private bool _showShareDialog;
    private string? _qrCodeImageAsBase64; 
    private string? _shareLink;

    // request dialog 
    private DialogOptions _requestDialogOptions = new() { MaxWidth = MaxWidth.Small, DisableBackdropClick = true };
    private bool _showRequestDialog;
    //private string? _firstName;
    //private string? _lastName;
    //private string? _emailAddress;
    //private string? _phoneNumber;
    //private string? _phoneExtension;
    //private string? _messageText;
    //private bool _validRequestInputs;

    //protected override async Task OnInitializedAsync()
    //{
    //}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // need to make sure the component is rendered before javascript calls can be made
        if (firstRender)
        {
            try
            {
                _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(SubdirectoryName);
                _document = await DataServicesManager.CadDriveService.GetDriveDocumentByGuid(System.Guid.Parse(Guid));
            }
            catch
            {
                NavigationManager.NavigateTo("/notfound");
                return;
            }

            // TO DO: may want to create a sharable link for each specific configuration
            _shareLink = $"{NavigationManager.BaseUri}catalog/{_subscription?.SubdirectoryName}/{_document?.Guid}";
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(_shareLink, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new(qrCodeData);
            _qrCodeImageAsBase64 = qrCode.GetGraphic(20);

            await _forgeViewer!.ViewDocument(_document!.BucketKey, _document!.ObjectKey);

            _initializing = false;
            StateHasChanged();
        }
    }

    //private async Task RequestQuote_OnClick()
    //{
        //if (!_validRequestInputs) 
        //    return;

        //if (_product == null || _productConfiguration == null) 
        //    return;

        //// create new record in db
        //await _dataServicesManager.ProductService.CreateProductQuoteRequest(productConfigurationId: _productConfiguration.Id,
        //                                                                    firstName: _firstName,
        //                                                                    lastName: _lastName,
        //                                                                    emailAddress: _emailAddress,
        //                                                                    phoneNumber: _phoneNumber,
        //                                                                    phoneExtension: _phoneExtension,
        //                                                                    messageText: _messageText);

        //// send email notification to subscribers of this event
        //ProductQuoteRequestEmailModel model = new()
        //{
        //    CustomerName = $"{_firstName} {_lastName}",
        //    ProductName = _product.DisplayName
        //};

        //_ = _emailService.SendNotificationEmail(subscriptionId: _subscription!.Id,
        //                                        notificationId: (int)NotificationIdEnum.ProductQuoteRequest_Insert,
        //                                        subject: "New Request!",
        //                                        emailTemplatePath: model.Path,
        //                                        emailModel: model);

        //_snackbar.Add("Request submitted!", Severity.Success);
        //_showRequestDialog = false;
    //}

    private async Task Download_OnClick()
    {
        string url = await ForgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_document!.BucketKey, _document!.ObjectKey);
        NavigationManager.NavigateTo(url);
    }

}
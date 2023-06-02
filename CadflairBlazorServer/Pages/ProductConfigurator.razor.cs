using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using QRCoder;

namespace CadflairBlazorServer.Pages
{
    public partial class ProductConfigurator : IAsyncDisposable
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager  _forgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;
        [Inject] EmailService _emailService { get; set; } = default!;
        [Inject] IDialogService  _dialogService { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] IJSRuntime _js { get; set; } = default!;

        // parameters
        [Parameter] public string ProductName { get; set; } = string.Empty;
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private ForgeViewer? _forgeViewer;
        private HubConnection? _hubConnection;
        private Subscription? _subscription;
        private Product? _product;
        private ProductVersion? _productVersion;
        private ProductConfiguration? _defaultConfiguration;
        private ProductConfiguration? _productConfiguration;
        private ILogicFormElement? _iLogicFormData;
        private bool _showSubmitButton = true;
        private bool _showRequestButton = false;
        private bool _validConfigurationInputs = false;
        private bool _configurationInProgress = false;
        private bool _initializing = true;

        // dialog fields
        private DialogOptions _shareDialogOptions = new() { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };
        private bool _showShareDialog;
        private string? _qrCodeImageAsBase64; 
        private string? _shareLink;

        private DialogOptions _requestDialogOptions = new() { MaxWidth = MaxWidth.Large, DisableBackdropClick = true };
        private bool _showRequestDialog;
        private string? _firstName;
        private string? _lastName;
        private string? _emailAddress;
        private string? _phoneNumber;
        private string? _phoneExtension;
        private string? _messageText;
        private bool _validRequestInputs;


        protected override async Task OnInitializedAsync()
        {
            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
            _product = await _dataServicesManager.ProductService.GetProductBySubscriptionIdAndSubdirectoryName(_subscription.Id, ProductName);
            _productVersion = await _dataServicesManager.ProductService.GetLatestProductVersionByProductId(_product.Id);
            _defaultConfiguration = await _dataServicesManager.ProductService.GetDefaultProductConfigurationByProductVersionId(_productVersion.Id);

            if (_subscription == null || _product == null || _productVersion == null || _defaultConfiguration == null)
            {
                _navigationManager.NavigateTo("/notfound");
                return;
            }

            // setup signal R hub connection
            _hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/forgecallbackhub"))
                                                       .WithAutomaticReconnect()
                                                       .Build();

            _hubConnection.On<string>(nameof(ForgeCallbackController.CreateProductConfigurationModel_OnProgress), ReportProgress);
            _hubConnection.On<int>(nameof(ForgeCallbackController.CreateProductConfigurationModel_OnComplete), ProductConfigurationCreated);
            _hubConnection.On<string>(nameof(ForgeCallbackController.ModelDerivativeTranslation_OnComplete), ShowProductConfiguration);

            // construct UI
            if(_productVersion.ILogicFormJson != null)
            {
                _iLogicFormData = JsonConvert.DeserializeObject<ILogicFormElement>(_productVersion.ILogicFormJson)!;
                _iLogicFormData.SetParameterExpressions(_defaultConfiguration.ArgumentJson);
            }

            // TO DO: may want to create a sharable link for each specific configuration
            _shareLink = $"{_navigationManager.BaseUri}{_subscription?.SubdirectoryName}/products/{_product?.SubdirectoryName}";
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(_shareLink, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new(qrCodeData);
            _qrCodeImageAsBase64 = qrCode.GetGraphic(20);

            _initializing = false;

            // load default model in viewer
            await _forgeViewer!.ViewDocument(_product!.ForgeBucketKey, _defaultConfiguration.ForgeZipKey);
            StateHasChanged();
        }

        private async Task Submit_OnClick()
        {
            if (_configurationInProgress)
                return;

            _configurationInProgress = true;

            //check for existing configuration
            List<ProductConfiguration> existingConfigurations = await _dataServicesManager.ProductService.GetProductsConfigurationsByProductVersionId(_productVersion!.Id);
            _productConfiguration = existingConfigurations.Find(i => i.ArgumentJson == _iLogicFormData?.GetArgumentJson());

            if (_productConfiguration != null)
            {
                _snackbar.Add("Existing configuration found!", Severity.Info);
                _configurationInProgress = false;
                _showSubmitButton = false;
                _showRequestButton = true;
                _ = _forgeViewer!.ViewDocument(_product!.ForgeBucketKey, _productConfiguration.ForgeZipKey);
                StateHasChanged();
                return;
            }


            // connect to Signal R hub
            if (_hubConnection?.State != HubConnectionState.Connected)
                await _hubConnection?.StartAsync()!;

            _snackbar.Add("Generating new configuration", Severity.Info);


            // create record in database
            ProductConfiguration newConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productVersionId: _productVersion.Id,
                                                                                                                         argumentJson: _iLogicFormData!.GetArgumentJson(),
                                                                                                                         forgeZipKey: null,
                                                                                                                         isDefault: false);

            // submit the request to design automation 
            await _forgeServicesManager.DesignAutomationService.CreateProductConfigurationModel(connectionId: _hubConnection?.ConnectionId!,
                                                                                                productConfigurationId: newConfiguration.Id,
                                                                                                inputBucketKey: _product!.ForgeBucketKey,
                                                                                                inputObjectKey: _defaultConfiguration!.ForgeZipKey,
                                                                                                inputPathInZip: _productVersion.RootFileName,
                                                                                                inventorParamsJson: _iLogicFormData.GetArgumentJson());
        }

        private void ReportProgress(string message) => _snackbar.Add(message, Severity.Info);

        private async Task ProductConfigurationCreated(int productConfigurationId)
        {
            _snackbar.Add("Generating preview...", Severity.Info);
            _productConfiguration = await _dataServicesManager.ProductService.GetProductConfigurationById(productConfigurationId);
        }

        private void ShowProductConfiguration(string urn)
        {
            _configurationInProgress = false;
            _showSubmitButton = false;
            _showRequestButton = true;
            _snackbar.Add("Configuration generated successfully!", Severity.Info);
            _ = _forgeViewer!.ViewDocument(urn);
            InvokeAsync(StateHasChanged);
        }

        private async Task RequestQuote_OnClick()
        {
            if (!_validRequestInputs) 
                return;

            if (_product == null || _productConfiguration == null) 
                return;

            // create new record in db
            await _dataServicesManager.ProductService.CreateProductQuoteRequest(productConfigurationId: _productConfiguration.Id,
                                                                                firstName: _firstName,
                                                                                lastName: _lastName,
                                                                                emailAddress: _emailAddress,
                                                                                phoneNumber: _phoneNumber,
                                                                                phoneExtension: _phoneExtension,
                                                                                messageText: _messageText);

            // send email notification to subscribers of this event
            ProductQuoteRequestEmailModel model = new()
            {
                CustomerName = $"{_firstName} {_lastName}",
                ProductName = _product.DisplayName
            };

            _ = _emailService.SendNotificationEmail(subscriptionId: _subscription!.Id,
                                                    notificationId: (int)NotificationIdEnum.ProductQuoteRequest_Insert,
                                                    subject: "New Request!",
                                                    emailTemplatePath: model.Path,
                                                    emailModel: model);

            _snackbar.Add("Request submitted!", Severity.Success);
            _showRequestDialog = false;
        }

        private async Task DownloadStp_OnClick()
        {
            if (_productConfiguration?.ForgeStpKey == null)
                return;

            string url = await _forgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_product!.ForgeBucketKey, _productConfiguration.ForgeStpKey);
            _navigationManager.NavigateTo(url);
        }

        private async Task DownloadZip_OnClick()
        {
            if (_productConfiguration?.ForgeZipKey == null)
                return;

            string url = await _forgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_product!.ForgeBucketKey, _productConfiguration.ForgeZipKey);
            _navigationManager.NavigateTo(url);
        }

        private void StartOver_OnClick()
        {
            _showRequestButton = false;
            _showSubmitButton = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
                await _hubConnection.DisposeAsync();
        }
    }
}
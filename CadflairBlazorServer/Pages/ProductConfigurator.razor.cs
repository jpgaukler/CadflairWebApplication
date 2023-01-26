using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using MudBlazor;
using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
using CadflairBlazorServer.Shared.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using FluentEmail.Core.Models;
using System.Diagnostics;
using FluentEmail.Core;
using CadflairBlazorServer.Helpers;
using CadflairBlazorServer.Shared.Dialogs;

namespace CadflairBlazorServer.Pages
{
    public partial class ProductConfigurator : IAsyncDisposable
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager  _forgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;
        [Inject] IFluentEmail _emailService { get; set; } = default!;
        [Inject] IDialogService  _dialogService { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] IJSRuntime _js { get; set; } = default!;

        // parameters
        [Parameter] public string ProductName { get; set; } = string.Empty;
        [Parameter] public string CompanyName { get; set; } = string.Empty;

        // fields
        private ForgeViewer? _forgeViewer;
        private HubConnection? _hubConnection;
        private Subscription _subscription = default!;
        private Product _product = default!;
        private ProductVersion _productVersion = default!;
        private ProductConfiguration _defaultConfiguration = default!;
        private ProductConfiguration? _productConfiguration;
        private ILogicFormElement _iLogicFormData = default!;
        private bool _validInputs = false;
        private bool _configurationInProgress = false;

        protected override async Task OnInitializedAsync()
        {
            // setup signal R hub connection
            _hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/forgecallbackhub"))
                                                       .WithAutomaticReconnect()
                                                       .Build();

            _hubConnection.On<string>("CreateProductConfigurationModel_OnProgress", ReportProgress);
            _hubConnection.On<int>("CreateProductConfigurationModel_OnComplete", ProductConfigurationCreated);
            _hubConnection.On<string>("ModelDerivativeTranslation_OnComplete", ShowProductConfiguration);

            // get data
            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
            _product = await _dataServicesManager.ProductService.GetProductBySubscriptionIdAndSubdirectoryName(_subscription.Id, ProductName);
            _productVersion = await _dataServicesManager.ProductService.GetLatestProductVersionByProductId(_product.Id);
            _defaultConfiguration = await _dataServicesManager.ProductService.GetDefaultProductConfigurationByProductVersionId(_productVersion.Id);

            // construct UI
            _iLogicFormData = JsonConvert.DeserializeObject<ILogicFormElement>(_productVersion.ILogicFormJson)!;
            _iLogicFormData.SetParameterExpressions(_defaultConfiguration.ArgumentJson);
            await _forgeViewer!.ViewDocument(_product.ForgeBucketKey, _defaultConfiguration.ForgeZipKey);
            StateHasChanged();
        }

        private async Task Submit_OnClick()
        {
            //check for existing configuration
            List<ProductConfiguration> existingConfigurations = await _dataServicesManager.ProductService.GetProductsConfigurationsByProductVersionId(_productVersion.Id);
            _productConfiguration = existingConfigurations.Find(i => i.ArgumentJson == _iLogicFormData.GetArgumentJson());

            if(_productConfiguration != null)
            {
                _snackbar.Add("Existing configuration found", Severity.Info);
                _ = _forgeViewer!.ViewDocument(_product.ForgeBucketKey, _productConfiguration.ForgeZipKey);
                StateHasChanged();
                return;
            }

            if (_configurationInProgress) return;
            _configurationInProgress = true;

            // connect to Signal R hub
            if (_hubConnection?.State != HubConnectionState.Connected) await _hubConnection?.StartAsync()!;

            _snackbar.Add("Generating new configuration", Severity.Info);


            // create record in database
            ProductConfiguration newConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productVersionId: _productVersion.Id, 
                                                                                                                         argumentJson: _iLogicFormData.GetArgumentJson(), 
                                                                                                                         forgeZipKey: null, 
                                                                                                                         isDefault: false);

            // submit the request to design automation 
            await _forgeServicesManager.DesignAutomationService.CreateProductConfigurationModel(connectionId: _hubConnection?.ConnectionId!, 
                                                                                                productConfigurationId: newConfiguration.Id, 
                                                                                                inputBucketKey: _product.ForgeBucketKey, 
                                                                                                inputObjectKey: _defaultConfiguration.ForgeZipKey, 
                                                                                                inputPathInZip: _productVersion.RootFileName, 
                                                                                                inventorParamsJson: _iLogicFormData.GetArgumentJson());
        }

        private void ReportProgress(string message)
        {
            _snackbar.Add(message, Severity.Info);
        }

        private async Task ProductConfigurationCreated(int productConfigurationId)
        {
            _snackbar.Add("Generating preview...", Severity.Info);
            _productConfiguration = await _dataServicesManager.ProductService.GetProductConfigurationById(productConfigurationId);
        }

        private void ShowProductConfiguration(string urn)
        {
            _configurationInProgress = false;
            _snackbar.Add("Configuration generated successfully!", Severity.Info);
            _ = _forgeViewer!.ViewDocument(urn);
            InvokeAsync(StateHasChanged);
        }

        private async Task RequestQuote_OnClick()
        {
            if (_productConfiguration == null) return;

            DialogParameters parameters = new()
            {
                { "Product", _product },
                { "ProductVersion", _productVersion },
                { "ProductConfiguration", _productConfiguration }
            };

            DialogOptions options = new()
            {
                CloseButton = true,
                CloseOnEscapeKey = false,
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Large
            };

            var result = await _dialogService.Show<ProductQuoteRequestDialog>($"Request a Quote - {_product.DisplayName}", parameters, options).Result;

            if (!result.Canceled)
            {
                ProductQuoteRequestDialog dialog = (ProductQuoteRequestDialog)result.Data;

                // create new record in db
                await _dataServicesManager.ProductService.CreateProductQuoteRequest(productConfigurationId: _productConfiguration.Id,
                                                                                    firstName: dialog.FirstName,
                                                                                    lastName: dialog.LastName,
                                                                                    emailAddress: dialog.EmailAddress,
                                                                                    phoneNumber: dialog.PhoneNumber,
                                                                                    phoneExtension: dialog.PhoneExtension,
                                                                                    messageText: dialog.MessageText);

                _ = _emailService.SendNotificationEmail(dataServicesManager: _dataServicesManager,
                                                        subscriptionId: _subscription.Id,
                                                        eventName: "ProductQuoteRequest_Insert",
                                                        subject: "New Request!",
                                                        templateFilename: "NewProductQuoteRequest.cshtml",
                                                        model: new { CustomerName = $"{dialog.FirstName} {dialog.LastName}", ProductName = _product.DisplayName }); 

                _snackbar.Add("Request submitted!", Severity.Success);
            }
        }

        private async Task DownloadStp_OnClick()
        {
            if (_productConfiguration?.ForgeStpKey == null) return;
            string url = await _forgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_product.ForgeBucketKey, _productConfiguration.ForgeStpKey);
            _navigationManager.NavigateTo(url);
        }

        private async Task DownloadZip_OnClick()
        {
            if (_productConfiguration?.ForgeZipKey == null) return;
            string url = await _forgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_product.ForgeBucketKey, _productConfiguration.ForgeZipKey);
            _navigationManager.NavigateTo(url);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null) await _hubConnection.DisposeAsync();
        }
    }
}
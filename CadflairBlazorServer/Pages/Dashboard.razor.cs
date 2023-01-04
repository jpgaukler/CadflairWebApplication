using Newtonsoft.Json;
using CadflairDataAccess.Models;
using CadflairBlazorServer.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using CadflairDataAccess;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Authorization;

namespace CadflairBlazorServer.Pages
{
    public partial class Dashboard
    {
        // services
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager _forgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;


        // fields
        private User _loggedInUser = new();
        private Subscription _subscription = new();
        private List<Product> _products = new();
        private Product? _selectedProduct;
        private ProductVersion? _latestProductVersion;
        private ProductConfiguration? _defaultProductConfiguration;
        private List<ILogicFormElement> _parameterGridItems = new();
        private string _thumbnailString = string.Empty;
        private bool _showGetStarted = false;

        protected override async Task OnInitializedAsync()
        {
            _loggedInUser = await _authenticationStateProvider.GetUser(_dataServicesManager.UserService);

            if (_loggedInUser.SubscriptionId == null)
            {
                _showGetStarted = true;
                return;
            }

            _subscription = await _dataServicesManager.SubscriptionService.GetSubscriptionById((int)_loggedInUser.SubscriptionId!) ?? new();
        }

        private async Task SelectedProductChanged(Product? product)
        {
            if (product == null) return;

            // get data
            _selectedProduct = product;
            _latestProductVersion = await _dataServicesManager.ProductService.GetLatestProductVersionByProductId(_selectedProduct.Id);
            _defaultProductConfiguration = await _dataServicesManager.ProductService.GetDefaultProductConfigurationByProductVersionId(_latestProductVersion.Id);

            // refresh UI
            ILogicFormElement form = JsonConvert.DeserializeObject<ILogicFormElement>(_latestProductVersion.ILogicFormJson)!;
            form.SetParameterExpressions(_defaultProductConfiguration.ArgumentJson);
            _parameterGridItems = form.GetParameterList();
            _thumbnailString = await _forgeServicesManager.ModelDerivativeService.GetThumbnailBase64String(_selectedProduct.ForgeBucketKey, _defaultProductConfiguration.ForgeZipKey);

            //await _forgeViewer!.ViewDocument(_product.ForgeBucketKey, _defaultConfiguration.ForgeZipKey);
            //StateHasChanged();
            //_navigationManager.NavigateTo($"/{_subscription.SubdirectoryName}/products/{product.SubdirectoryName}");
            //_productConfigurations = await _dataServicesManager.ProductService.GetProductsConfigurationsByProductVersionId(latestVersion.Id);
        }

        private async Task SelectedProductFolderChanged(ProductFolder? productFolder)
        {
            if (productFolder == null) return;
            _products = await _dataServicesManager.ProductService.GetProductsByProductFolderId(productFolder.Id);
        }
    }
}
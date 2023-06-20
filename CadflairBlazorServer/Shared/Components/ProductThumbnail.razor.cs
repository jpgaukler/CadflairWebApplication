using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ProductThumbnail
    {
        // services
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager _forgeServicesManager { get; set; } = default!;

        // parameters

        /// <summary>
        /// The desired height of the thumbnail. Possible values are 100, 200 and 400.
        /// </summary>
        [Parameter] public int Height { get; set; } = 100;

        /// <summary>
        /// The desired width of the thumbnail. Possible values are 100, 200 and 400.
        /// </summary>
        [Parameter] public int Width { get; set; } = 100;

        /// <summary>
        /// Product to get the thumbnail for.
        /// </summary>
        [Parameter] public Product? Product { get; set; }

        /// <summary>
        /// Configuration to get the thumbnail for. If not configuration if provided, the default configuration of the latest product version will be used.
        /// </summary>
        [Parameter] public ProductConfiguration? ProductConfiguration { get; set; }

        // fields
        private string _thumbnailStringBase64 = string.Empty;
        private bool _loading = false;

        protected override async Task OnInitializedAsync()
        {
            if (Product == null)
                return;

            _loading = true;

            if (ProductConfiguration == null)
            {
                // get the default configuration
                ProductVersion latestVersion = await _dataServicesManager.ProductService.GetLatestProductVersionByProductId(Product.Id);
                ProductConfiguration = await _dataServicesManager.ProductService.GetDefaultProductConfigurationByProductVersionId(latestVersion.Id);
            }

            //if (await _forgeServicesManager.ModelDerivativeService.TranslationExists(Product.ForgeBucketKey, ProductConfiguration.ForgeZipKey))
            //{
            //    _thumbnailStringBase64 = await _forgeServicesManager.ModelDerivativeService.GetThumbnailBase64String(Product.ForgeBucketKey, ProductConfiguration.ForgeZipKey, Width, Height);
            //}

            _loading = false;
            StateHasChanged();
        }
    }
}
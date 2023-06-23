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

        //protected override async Task OnInitializedAsync()
        protected override async Task OnParametersSetAsync()
        {
            if (Product == null)
                return;

            _loading = true;
            StateHasChanged();

            // get the default configuration
            // MAY NEED TO MODIFY THIS IF I WANT TO SHOT A DIFFERENT CONFIGURATION
            ProductVersion latestVersion = await _dataServicesManager.ProductService.GetLatestProductVersionByProductId(Product.Id);
            ProductConfiguration = await _dataServicesManager.ProductService.GetDefaultProductConfigurationByProductVersionId(latestVersion.Id);
            _thumbnailStringBase64 = await _forgeServicesManager.ObjectStorageService.GetThumbnailAsBase64(ProductConfiguration.BucketKey);

            _loading = false;
        }

    }
}
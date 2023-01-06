using Microsoft.AspNetCore.Components;
using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;

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
        /// Product to get the thumbnail for. The default configuration of the latest product version will be used.
        /// </summary>
        [Parameter] public Product Product
        {
            set
            {
                if (_product?.Id == value?.Id) return;
                _product = value;
                _ = LoadThumnailImage();
            }
        }

        // fields
        private Product? _product;
        private string? _thumbnailStringBase64;
        private bool _loading = false;


        private async Task LoadThumnailImage()
        {
            if(_product == null) return;

            _thumbnailStringBase64 = null;
            _loading = true;

            ProductVersion version = await _dataServicesManager.ProductService.GetLatestProductVersionByProductId(_product.Id);
            ProductConfiguration defaultConfiguration = await _dataServicesManager.ProductService.GetDefaultProductConfigurationByProductVersionId(version.Id);

            if (await _forgeServicesManager.ModelDerivativeService.TranslationExists(_product.ForgeBucketKey, defaultConfiguration.ForgeZipKey))
            {
                _thumbnailStringBase64 = await _forgeServicesManager.ModelDerivativeService.GetThumbnailBase64String(_product.ForgeBucketKey, defaultConfiguration.ForgeZipKey, Width, Height);
            }

            _loading = false;
            StateHasChanged();
        }

    }
}
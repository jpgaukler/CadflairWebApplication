using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class CatalogModelThumbnail
    {
        // services
        [Inject] ForgeServicesManager ForgeServicesManager { get; set; } = default!;

        // parameters
        [Parameter] public int Height { get; set; } = 100;
        [Parameter] public int Width { get; set; } = 100;
        [Parameter] public CatalogModel? CatalogModel { get; set; }

        // fields
        private string _thumbnailStringBase64 = string.Empty;
        private bool _loading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (CatalogModel == null)
                return;

            _loading = true;
            StateHasChanged();

            _thumbnailStringBase64 = await ForgeServicesManager.ModelDerivativeService.GetThumbnailBase64String(CatalogModel.BucketKey, CatalogModel.ObjectKey, 400, 400);
            _loading = false;
        }

    }
}
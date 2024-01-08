using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class DriveDocumentThumbnail
{
    // services
    [Inject] ForgeServicesManager ForgeServicesManager { get; set; } = default!;

    // parameters
    [Parameter] public int Height { get; set; } = 100;
    [Parameter] public int Width { get; set; } = 100;
    [Parameter] public DriveDocument? Document { get; set; }

    // fields
    private string _thumbnailStringBase64 = string.Empty;
    private bool _loading = false;

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        //_thumbnailStringBase64 = string.Empty;
        //StateHasChanged();

        if (Document != null)
            _thumbnailStringBase64 = await ForgeServicesManager.ModelDerivativeService.GetThumbnailBase64String(Document.BucketKey, Document.ObjectKey, 400, 400);

        _loading = false;
    }

}
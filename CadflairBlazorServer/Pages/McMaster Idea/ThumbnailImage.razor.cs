using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class ThumbnailImage
    {
        // services
        [Inject] DataServicesManager DataServicesManager { get; set; } = default!;
        [Inject] BlobServiceClient BlobServiceClient { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;

        // parameters
        [Parameter] public string? Uri { get; set; }
        [Parameter] public EventCallback<string?> UriUpdated { get; set; }
        /// <summary>
        /// Height in pixels. Default is 100.
        /// </summary>
        [Parameter] public int Height { get; set; } = 100;
        /// <summary>
        /// Width in pixels. Default is 100.
        /// </summary>
        [Parameter] public int Width { get; set; } = 100;
        /// <summary>
        /// Flag to determine if the component should allow the thumbnail image to be edited.
        /// </summary>
        [Parameter] public bool EditMode { get; set; } = false;


        // fields
        private const long MAX_THUMBNAIL_SIZE_IN_BYTES = 10 * 1024 * 1024;
        private readonly string[] _validExtensions = { ".jpg", ".jpeg", ".png", ".svg", "gif" };
        private bool _loading = false;

        private const string _initialDragStyle = $"border-color: var(--mud-palette-lines-inputs);";
        private string _dragStyle = _initialDragStyle;
        private void SetDragStyle() => _dragStyle = "border-color: var(--mud-palette-primary)!important";
        private void ClearDragStyle() => _dragStyle = _initialDragStyle;

        private async Task UploadThumbnail(IBrowserFile file)
        {
            if (!_validExtensions.Any(i => i == Path.GetExtension(file.Name)))
            {
                Snackbar.Add("Invalid file extension!", Severity.Warning); 
                return;
            }

            _loading = true;

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name);
            BlobContainerClient containerClient = BlobServiceClient.GetBlobContainerClient("images");
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using Stream stream = file.OpenReadStream(MAX_THUMBNAIL_SIZE_IN_BYTES);
            await blobClient.UploadAsync(stream);
            await UriUpdated.InvokeAsync(blobClient.Uri.ToString());

            _loading = false;
        }

        private async Task DeleteThumbnail()
        {
            if (Uri == null)
                return;

            BlobUriBuilder blobUriBuilder = new(new Uri(Uri));
            BlobContainerClient containerClient = BlobServiceClient.GetBlobContainerClient("images");
            BlobClient blobClient = containerClient.GetBlobClient(blobUriBuilder.BlobName);
            await blobClient.DeleteAsync();
            await UriUpdated.InvokeAsync(null);
        }

    }
}
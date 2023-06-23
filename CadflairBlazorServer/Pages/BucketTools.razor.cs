using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CadflairBlazorServer.Pages
{
    public partial class BucketTools
    {
        // services

        [Inject] ProtectedSessionStorage _protectedSessionStorage { get; set; } = default!;
        [Inject] DataServicesManager _dataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager _forgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager _navigationManager { get; set; } = default!;

        // parameters

        // fields
        List<string> _buckets = new();

        protected override async Task OnInitializedAsync()
        {
            _buckets = await _forgeServicesManager.ObjectStorageService.GetBucketKeys();
        }

        private async Task DeleteBucket_OnClick(string bucketKey)
        {
            await _forgeServicesManager.ObjectStorageService.DeleteBucket(bucketKey);
            _buckets.Remove(bucketKey);
        }


    }
}
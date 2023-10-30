using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class ForgeViewer
    {
        // services
        [Inject] IJSRuntime Js { get; set; } = default!;
        [Inject] ILogger<ForgeViewer> Logger { get; set; } = default!;
        [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;

        // fields
        private bool _modelNotFound = false;

        public async Task ViewDocumentInOss(string bucketKey, string objectKey = "bubble.json")
        {
            _modelNotFound = false;

            try
            {
                //get public token for viewables
                var token = await ForgeServicesManager.AuthorizationService.GetPublic();

                var parameters = new
                {
                    Token = token.access_token,
                    BucketKey = bucketKey,
                    ObjectKey = objectKey,
                };

                //invoke the viewer
                await Js.InvokeVoidAsync("loadModelFromOss", parameters);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to load model from OSS - bucketKey: {bucketKey}, objectKey: {objectKey}");
                _modelNotFound = true;
            }

            StateHasChanged();
        }

        public async Task ViewDocument(string bucketKey, string objectKey)
        {
            _modelNotFound = false;

            try
            {
                //get public token for viewables
                var token = await ForgeServicesManager.AuthorizationService.GetPublic();

                //get forge object id
                dynamic forgeObject = await ForgeServicesManager.ObjectStorageService.GetObjectDetails(bucketKey, objectKey);

                if (await ForgeServicesManager.ModelDerivativeService.TranslationExists(forgeObject.encoded_urn))
                {
                    //define input parameters as JSON
                    var parameters = new
                    {
                        Token = token.access_token,
                        Urn = forgeObject.encoded_urn,
                    };

                    //invoke the viewer
                    await Js.InvokeVoidAsync("loadModelFromUrn", parameters);
                }
                else
                {
                    Logger.LogWarning($"Model derivative manifest not found - bucketKey: {bucketKey}, objectKey: {objectKey}");
                    _modelNotFound = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to load model from derivative - bucketKey: {bucketKey}, objectKey: {objectKey}");
                _modelNotFound = true;
            }

            StateHasChanged();
        }

    }
}
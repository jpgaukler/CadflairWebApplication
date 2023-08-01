using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class ForgeViewer
    {
        [Inject] IJSRuntime _js { get; set; } = default!;
        [Inject] ILogger<ForgeViewer> _logger { get; set; } = default!;
        [Inject] ForgeServicesManager  _forgeServicesManager { get; set; } = default!;

        private bool _modelNotFound = false;

        public async Task ViewDocumentInOss(string bucketKey, string objectKey = "bubble.json")
        {
            _modelNotFound = false;

            try
            {
                //get public token for viewables
                var token = await _forgeServicesManager.AuthorizationService.GetPublic();

                var parameters = new
                {
                    Token = token.access_token,
                    BucketKey = bucketKey,
                    ObjectKey = objectKey,
                };

                //invoke the viewer
                await _js.InvokeVoidAsync("loadModelFromOss", parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load model from OSS - bucketKey: {bucketKey}, objectKey: {objectKey}");
                _modelNotFound = true;
            }
        }

        public async Task ViewDocument(string bucketKey, string objectKey)
        {
            _modelNotFound = false;

            try
            {
                //get public token for viewables
                var token = await _forgeServicesManager.AuthorizationService.GetPublic();

                //get forge object id
                dynamic forgeObject = await _forgeServicesManager.ObjectStorageService.GetObjectDetails(bucketKey, objectKey);

                if (await _forgeServicesManager.ModelDerivativeService.TranslationExists(forgeObject.encoded_urn))
                {
                    //define input parameters as JSON
                    var parameters = new
                    {
                        Token = token.access_token,
                        Urn = forgeObject.encoded_urn,
                    };

                    //invoke the viewer
                    await _js.InvokeVoidAsync("loadModelFromUrn", parameters);
                }
                else
                {
                    _logger.LogWarning($"Model derivative not found - bucketKey: {bucketKey}, objectKey: {objectKey}");
                    _modelNotFound = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to load model from derivative - bucketKey: {bucketKey}, objectKey: {objectKey}");
                _modelNotFound = true;
            }
        }

    }
}
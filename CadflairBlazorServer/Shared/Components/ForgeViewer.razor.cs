using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ForgeViewer
    {
        [Inject] IJSRuntime _js { get; set; } = default!;
        [Inject] ForgeServicesManager  _forgeServicesManager { get; set; } = default!;
        [Inject] ILogger<ForgeViewer> _logger { get; set; } = default!;

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
                await _js.InvokeVoidAsync("launchViewer", parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load model from OSS - bucketKey: {bucketKey}, objectKey: {objectKey}");
                _modelNotFound = true;
            }
        }

        //public async Task ViewDocument(string encodedUrn)
        //{
        //    _modelNotFound = false;

        //    try
        //    {
        //        //get public token for viewables
        //        var token = await _forgeServicesManager.AuthorizationService.GetPublic();

        //        if (await _forgeServicesManager.ModelDerivativeService.TranslationExists(encodedUrn))
        //        {
        //            //define input parameters as JSON
        //            var parameters = new
        //            {
        //                Token = token.access_token,
        //                Urn = encodedUrn,
        //            };

        //            //invoke the viewer
        //            await _js.InvokeVoidAsync("launchViewer", parameters);
        //        }
        //        else
        //        {
        //            _logger.LogWarning($"ForgeViewer: Model derivative not found - Urn (base64): {encodedUrn}");
        //            _modelNotFound = true;

        //            //need to translate object
        //            //await _forgeServicesManager.ModelDerivativeService.TranslateObject(forgeObject.encoded_urn, "Dresser Configurator.ipt");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Failed to load model - urn: {encodedUrn}");
        //        _modelNotFound = true;
        //    }
        //}

        //public async Task ViewDocument(string bucketKey, string objectKey)
        //{
        //    _modelNotFound = false;

        //    //get forge object id
        //    dynamic forgeObject = await _forgeServicesManager.ObjectStorageService.GetObjectDetails(bucketKey, objectKey);
        //    ViewDocument(forgeObject.encoded_urn);
        //}

    }
}
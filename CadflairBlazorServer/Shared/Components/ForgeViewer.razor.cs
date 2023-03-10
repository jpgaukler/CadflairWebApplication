using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CadflairForgeAccess;
using System.Diagnostics;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class ForgeViewer
    {
        [Inject] IJSRuntime _js { get; set; } = default!;
        [Inject] ForgeServicesManager  _forgeServicesManager { get; set; } = default!;

        private bool _modelNotFound = false;

        public async Task ViewDocument(string encodedUrn)
        {
            //get public token for viewables
            var token = await _forgeServicesManager.AuthorizationService.GetPublic();

            if (await _forgeServicesManager.ModelDerivativeService.TranslationExists(encodedUrn))
            {
                //define input parameters as JSON
                var parameters = new
                {
                    Token = token.access_token,
                    Urn = encodedUrn
                };

                //invoke the viewer
                await _js.InvokeVoidAsync("launchViewer", parameters);
            }
            else
            {
                _modelNotFound = true;
                Trace.WriteLine($"ForgeViewer: Model derivative not found - Urn (base64): {encodedUrn}");

                //need to translate object
                //await _forgeServicesManager.ModelDerivativeService.TranslateObject(forgeObject.encoded_urn, "Dresser Configurator.ipt");
            }
        }

        public async Task ViewDocument(string bucketKey, string objectKey)
        {
            //get forge object id
            dynamic forgeObject = await _forgeServicesManager.ObjectStorageService.GetObjectDetails(bucketKey, objectKey);
            ViewDocument(forgeObject.encoded_urn);
        }
    }
}
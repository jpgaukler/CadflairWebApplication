using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Options;
using System.Diagnostics;


namespace CadflairForgeAccess.Services
{
    public class DesignAutomationService 
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly AuthorizationService _authService;
        private readonly ObjectStorageService _objectStorageService;

        public DesignAutomationService(AuthorizationService authService, ObjectStorageService objectStorageService)
        {
            _authService = authService;
            _objectStorageService = objectStorageService;

            // Create a new ForgeService to be used with the design automation client
            ForgeConfiguration forgeConfiguration = new()
            {
                ClientId = authService.ClientId,
                ClientSecret = authService.ClientSecret
            };

            ForgeHandler forgeHandler = new(Options.Create(forgeConfiguration))
            {
                InnerHandler = new HttpClientHandler()
            };

            HttpClient httpClient = new(forgeHandler);
            ForgeService forgeService = new(httpClient);

            _designAutomationClient = new(forgeService);
        }

        public async Task CreateProductConfigurationModel(string connectionId, int productConfigurationId, string inputBucketKey, string inputObjectKey, string inputPathInZip, string inventorParamsJson)
        {
            try
            {
                dynamic token = await _authService.GetInternal();

                // input file 
                XrefTreeArgument inputModelArgument = new()
                {
                    Url = await _objectStorageService.GetSignedDownloadUrl(inputBucketKey, inputObjectKey),
                    PathInZip = inputPathInZip,
                };

                // inventor params 
                XrefTreeArgument inventorParamsArgument = new()
                {
                    Url = $"data:application/json,{inventorParamsJson}",
                    LocalName = "params.json"
                };

                // output files
                string outputObjectKey = Guid.NewGuid().ToString() + ".zip";
                XrefTreeArgument outputModelArgument = new()
                {
                    Url = await _objectStorageService.GetSignedUploadUrl(inputBucketKey, outputObjectKey),
                    Verb = Verb.Put,
                };

                string outputStpKey = outputObjectKey.Replace(".zip", ".stp");
                XrefTreeArgument outputStpArgument = new()
                {
                    Url = await _objectStorageService.GetSignedUploadUrl(inputBucketKey, outputStpKey),
                    Verb = Verb.Put,
                };

                // callback urls 
                string callbackUrl = "https://a5b1-2601-88-301-97a0-3047-5b4b-cc66-ab71.ngrok.io";

                XrefTreeArgument onCompleteCallback = new()
                {
                    Verb = Verb.Post,
                    Url = $"{callbackUrl}/api/forge/designautomation/productconfiguration/create/oncomplete?connectionId={connectionId}&productConfigurationId={productConfigurationId}&outputBucketKey={inputBucketKey}&outputObjectKey={outputObjectKey}&rootFileName={inputPathInZip}&outputStpKey={outputStpKey}"
                };

                XrefTreeArgument onProgressCallback = new()
                {
                    Verb = Verb.Post,
                    Url = $"{callbackUrl}/api/forge/designautomation/productconfiguration/create/onprogress?connectionId={connectionId}"
                };

                // submit workitem 
                WorkItem workItemSpec = new()
                {
                    ActivityId = "cadflair.CreateProductConfiguration+v1",
                    Arguments = new Dictionary<string, IArgument>()
                    {
                        { "inputProduct", inputModelArgument },
                        { "inventorParams", inventorParamsArgument },
                        { "outputProduct", outputModelArgument },
                        { "outputStp", outputStpArgument },
                        { "onComplete", onCompleteCallback },
                        { "onProgress", onProgressCallback }
                    }
                };

                await _designAutomationClient.CreateWorkItemAsync(workItemSpec);

                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return;
            }
        }

    }


}

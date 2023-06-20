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
        private readonly string _callbackUrl;

        public DesignAutomationService(AuthorizationService authService, ObjectStorageService objectStorageService, string callbackUrl)
        {
            _authService = authService;
            _objectStorageService = objectStorageService;
            _callbackUrl = callbackUrl;

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
                string outputBucketKey = Guid.NewGuid().ToString();
                await _objectStorageService.CreateBucket(outputBucketKey);

                string outputZipKey = outputBucketKey + ".zip";
                XrefTreeArgument outputModelArgument = new()
                {
                    Url = await _objectStorageService.GetSignedUploadUrl(outputBucketKey, outputZipKey),
                    Verb = Verb.Put,
                };

                string outputStpKey = outputBucketKey + ".stp";
                XrefTreeArgument outputStpArgument = new()
                {
                    Url = await _objectStorageService.GetSignedUploadUrl(outputBucketKey, outputStpKey),
                    Verb = Verb.Put,
                };

                XrefTreeArgument outputSvfArgument = new()
                {
                    Verb = Verb.Post,
                    Url = $"{_callbackUrl}/api/v1/designautomation/productconfiguration/create/onsvfoutput/{outputBucketKey}",
                };

                // callback urls 
                XrefTreeArgument onCompleteCallback = new()
                {
                    Verb = Verb.Post,
                    Url = $"{_callbackUrl}/api/v1/designautomation/productconfiguration/create/oncomplete/{connectionId}/{productConfigurationId}/{outputBucketKey}/{outputZipKey}/{outputStpKey}"
                };

                XrefTreeArgument onProgressCallback = new()
                {
                    Verb = Verb.Post,
                    Url = $"{_callbackUrl}/api/v1/designautomation/productconfiguration/create/onprogress/{connectionId}"
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
                        { "outputSvf", outputSvfArgument },
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

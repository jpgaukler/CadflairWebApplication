using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task<string> CreateProductConfigurationModel(int productConfigurationId, string inputBucketKey, string inputObjectKey, string inputPathInZip, string inventorParamsJson)
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

                string outputObjectKey = Guid.NewGuid().ToString();

                XrefTreeArgument outputModelArgument = new()
                {
                    Url = await _objectStorageService.GetSignedUploadUrl(inputBucketKey, outputObjectKey),
                    Verb = Verb.Put,
                };

                // callback urls 
                string callbackUrl = "https://f972-216-164-179-107.ngrok.io";
                string connectionId = " 1234";

                XrefTreeArgument onCompleteCallback = new()
                {
                    Verb = Verb.Post,
                    Url = $"{callbackUrl}/api/forge/designautomation/productconfiguration/create/oncomplete?connectionId={connectionId}&productConfigurationId={productConfigurationId}&outputBucketKey={inputBucketKey}&outputObjectKey={outputObjectKey}&rootFileName={inputPathInZip}"
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
                        { "onComplete", onCompleteCallback },
                        { "onProgress", onProgressCallback }
                    }
                };

                WorkItemStatus workItemStatus = await _designAutomationClient.CreateWorkItemAsync(workItemSpec);

                Debug.WriteLine($"Creating product configuration -  Workitem Id: {workItemStatus.Id}, Status: {workItemStatus.Status}");

                return workItemStatus.Status.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error submitting workitem: {ex}");
                return $"Error submitting workitem: {ex}";
            }
        }

    }


}

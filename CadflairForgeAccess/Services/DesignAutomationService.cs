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

        public DesignAutomationService(AuthorizationService authService)
        {
            _authService = authService;

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

        public async Task<WorkItemStatus?> GenerateProductConfiguration(string productBucketKey, string productObjectKey, string productPathInZip, string inventorParams)
        {
            try
            {
                Debug.WriteLine($"Generating product configuration...");

                dynamic token = await _authService.GetInternal();

                Debug.WriteLine($"token: {token.access_token}");

                //--------------------------------------------------------------- input file ---------------------------------------------------------------------
                string workingFolderName = "NewModel"; //this is the name of the folder where the input files will be unzipped to on the InventorServer machine
                
                XrefTreeArgument inputModelArgument = new()
                {
                    Url = $"https://developer.api.autodesk.com/oss/v2/buckets/{productBucketKey}/objects/{productObjectKey}",
                    Verb = Verb.Get,
                    LocalName = workingFolderName,
                    PathInZip = productPathInZip,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", "Bearer " + token.access_token }
                    }
                };

                //--------------------------------------------------------------- inventor params ---------------------------------------------------------------------
                JObject paramsJson = JObject.Parse(inventorParams);
                Debug.WriteLine($"inventorParams: {inventorParams}");

                //quotes are getting replaced with single quotes, does this matter?

                XrefTreeArgument inventorParamsArgument = new()
                {
                    Url = $"data:application/json,{paramsJson.ToString(Formatting.None).Replace("\"", "'")}",
                    LocalName = "params.json"
                };

                Debug.WriteLine($"inventorParamsUrl: {inventorParamsArgument.Url}");

                //--------------------------------------------------------------- output files ---------------------------------------------------------------------

                //output zip of models pdf
                //string outputModelName = inputs.outputObjectKey + ".zip";
                string outputModelName = Guid.NewGuid().ToString();

                XrefTreeArgument outputModelArgument = new()
                {
                    Url = $"https://developer.api.autodesk.com/oss/v2/buckets/{productBucketKey}/objects/{outputModelName}",
                    Verb = Verb.Post,
                    LocalName = workingFolderName,
                    Headers = new Dictionary<string, string>()
                    {
                        {"Authorization", "Bearer " + token.access_token }
                    }
                };

                //--------------------------------------------------------------- callback url ---------------------------------------------------------------------
                XrefTreeArgument callbackUrlArgument = new()
                {
                    Verb = Verb.Post,
                    //Url = $"{Utils.GetAppSetting("FORGE_CALLBACK_URL")}/api/forge/designautomation/workitems/configuremodel/callback?connectionId={inputs.connectionId}&modelBucketKey={inputs.modelBucketKey}&modelObjectKey={outputModelName}&pathInZip={inputs.pathInZip}"
                    Url = $"/api/forge/designautomation/workitems/configuremodel/callback?connectionId"
                };



                //--------------------------------------------------------------- submit workitem ---------------------------------------------------------------------
                WorkItem workItemSpec = new()
                {
                    ActivityId = "cadflair.GenerateProductConfiguration+v1",
                    Arguments = new Dictionary<string, IArgument>()
                    {
                        { "inputProduct", inputModelArgument },
                        { "inventorParams", inventorParamsArgument },
                        { "outputProduct", outputModelArgument },
                        { "onComplete", callbackUrlArgument },
                        //{ "onProgress", callbackUrlArgument }
                    }
                };

                WorkItemStatus workItemStatus = await _designAutomationClient.CreateWorkItemAsync(workItemSpec);

                Debug.WriteLine($"status: {workItemStatus.Status}");
                Debug.WriteLine($"progress: {workItemStatus.Progress}");
                Debug.WriteLine($"report url: {workItemStatus.ReportUrl}");

                return workItemStatus;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }


        ///// <summary>
        ///// Callback from Design Automation Workitem (onComplete) 
        ///// </summary>
        //[HttpPost]
        //[Route("api/forge/designautomation/workitems/configuremodel/callback")]
        //public async Task<IActionResult> Workitem_OnCallback(string connectionId, string modelBucketKey, string modelObjectKey, string pathInZip, [FromBody] dynamic body)
        //{
        //    try
        //    {
        //        //parse json data
        //        JObject workItem = JObject.Parse((string)body.ToString());
        //        string status = workItem["status"].Value<string>();
        //        string reportUrl = workItem["reportUrl"].Value<string>();

        //        //download workitem report
        //        RestClient client = new RestClient(reportUrl);
        //        RestRequest request = new RestRequest(string.Empty);
        //        string reportTxt = Encoding.Default.GetString(client.DownloadData(request));

        //        await _hubContext.Clients.Client(connectionId).SendAsync("workItemComplete", workItem.ToString());
        //        await _hubContext.Clients.Client(connectionId).SendAsync("onProgress", reportTxt);

        //        if (status == "success")
        //        {
        //            //start translation if job was successful
        //            HttpResponseMessage requestTranslation = await ModelDerivativeService.TranslateObject(connectionId, modelBucketKey, modelObjectKey, pathInZip);
        //            await _hubContext.Clients.Client(connectionId).SendAsync("translationRequested", "Translation requested: " + requestTranslation.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    //ALWAYS RETURN OK TO THE FORGE API
        //    return Ok();
        //}



    }


}

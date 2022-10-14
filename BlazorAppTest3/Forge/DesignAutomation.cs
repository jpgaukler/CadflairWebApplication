using Autodesk.Forge;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkItem = Autodesk.Forge.DesignAutomation.Model.WorkItem;
using WorkItemStatus = Autodesk.Forge.DesignAutomation.Model.WorkItemStatus;


namespace BlazorAppTest3.Forge
{
    /// <summary>
    /// Class used by SignalR
    /// </summary>
    public class DesignAutomationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public string GetConnectionId() { return Context.ConnectionId; }
    }

    [ApiController]
    public class DesignAutomation : ControllerBase
    {
        #region Fields

        // Used to access the application folder (temp location for files & bundles)
        private readonly IWebHostEnvironment _env;

        // used to access the SignalR Hub
        private readonly IHubContext<DesignAutomationHub> _hubContext;

        // Design Automation v3 API
        private readonly DesignAutomationClient _designAutomation;


        #endregion

        #region Methods

        // Constructor, where env and hubContext are specified
        public DesignAutomation(IWebHostEnvironment env, IHubContext<DesignAutomationHub> hubContext, DesignAutomationClient api)
        {
            _designAutomation = api;
            _env = env;
            _hubContext = hubContext;
        }

        #endregion

        #region API

        /// <summary>
        /// Callback from Model Translation webhook. NEED TO MOVE THIS TO THE MODEL DERIVATIVE CONTROLLER
        /// </summary>
        [HttpPost]
        [Route("/api/forge/modelderivative/jobs/translationcomplete")]
        public async Task<IActionResult> TranslationComplete([FromBody] dynamic body)
        {
            try
            {
                JObject bodyJson = JObject.Parse((string)body.ToString());
                JObject? payload = bodyJson["payload"]?.Value<JObject>();
                JObject? workflowAttributes = payload["WorkflowAttributes"]?.Value<JObject>();

                string? resourceURN = bodyJson["resourceUrn"]?.Value<string>();
                string? id = workflowAttributes["connectionId"]?.Value<string>();

                await _hubContext.Clients.Client(id).SendAsync("onProgress", "translation completed: " + bodyJson.ToString());
                await _hubContext.Clients.Client(id).SendAsync("translationComplete", resourceURN);

                return Ok();
            }
            catch (Exception ex)
            {
            }

            // ALWAYS return ok (200)
            return Ok();
        }

        /// <summary>
        /// Input for StartWorkitem
        /// </summary>
        public class WorkItemInputs
        {
            public string connectionId { get; set; }
            public string inventorParams { get; set; }
            public string activityId { get; set; }
            public string inputBucketKey { get; set; }
            public string inputObjectKey { get; set; }
            public string pathInZip { get; set; }
            public string modelBucketKey { get; set; }
            public string pdfBucketKey { get; set; }
            public string stpBucketKey { get; set; }
            public string outputObjectKey { get; set; }
        }

        /// <summary>
        /// Start a new workitem 
        /// </summary>
        [HttpPost]
        [Route("api/forge/designautomation/workitems/createmodelconfiguration")]
        public async Task<IActionResult> ConfigureModel([FromForm] WorkItemInputs inputs)
        {
            try
            {
                dynamic oauth = OAuth.GetInternal();

                //--------------------------------------------------------------- input file ---------------------------------------------------------------------
                string workingFolderName = "NewModel"; //this is the name of the folder where the input files will be unzipped to on the InventorServer machine
                
                XrefTreeArgument inputModelArgument = new XrefTreeArgument()
                {
                    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputs.inputBucketKey, inputs.inputObjectKey),
                    Verb = Verb.Get,
                    LocalName = workingFolderName,
                    PathInZip = inputs.pathInZip,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", "Bearer " + oauth.access_token }
                    }
                };



                //--------------------------------------------------------------- inventor params ---------------------------------------------------------------------
                JObject inventorParams = JObject.Parse(inputs.inventorParams);

                XrefTreeArgument inventorParamsArgument = new XrefTreeArgument()
                {
                    Url = "data:application/json, " + inventorParams.ToString(Formatting.None).Replace("\"", "'"),
                    LocalName = "params.json"
                };


                //---------------------------------------------------------- check for duplicate object ---------------------------------------------------------------------

                ////uploading an object with the same name will overwrite the previous object. may want to account for this outside of the activity
                //dynamic existingObject = await OSSController.GetObjectDetailsAsync(inputs.modelBucketKey, inputs.outputObjectKey + ".zip");
                //if (existingObject != null)
                //{
                //    await _hubContext.Clients.Client(inputs.connectionId).SendAsync("onError", "An existing object was found: " + (string)existingObject.objectId);
                //    return Ok(new { Result = "Configuration already exists",  Urn = (string)existingObject.encodedURN }); ;
                //}


                //--------------------------------------------- create output buckets (in case they don't exist) ---------------------------------------------------------

                await ObjectStorage.CreateBucket(inputs.modelBucketKey);
                await ObjectStorage.CreateBucket(inputs.pdfBucketKey);
                await ObjectStorage.CreateBucket(inputs.stpBucketKey);

                //--------------------------------------------------------------- output files ---------------------------------------------------------------------

                //output zip of models pdf
                string outputModelName = inputs.outputObjectKey + ".zip";

                XrefTreeArgument outputModelArgument = new XrefTreeArgument()
                {
                    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputs.modelBucketKey, outputModelName),
                    Verb = Verb.Put,
                    LocalName = workingFolderName,
                    Headers = new Dictionary<string, string>()
                    {
                        {"Authorization", "Bearer " + oauth.access_token }
                    }
                };

                //drawing pdf
                string outputPdfName = inputs.outputObjectKey + ".pdf";

                XrefTreeArgument outputPdfArgument = new XrefTreeArgument()
                {
                    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputs.pdfBucketKey, outputPdfName),
                    Verb = Verb.Put,
                    LocalName = workingFolderName + "\\" + outputPdfName,
                    Headers = new Dictionary<string, string>()
                    {
                        {"Authorization", "Bearer " + oauth.access_token }
                    }
                };

                //model stp file
                string outputStpName = inputs.outputObjectKey + ".stp";

                XrefTreeArgument outputStpArgument = new XrefTreeArgument()
                {
                    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputs.stpBucketKey, outputStpName),
                    Verb = Verb.Put,
                    LocalName = workingFolderName + "\\" + outputStpName,
                    Headers = new Dictionary<string, string>()
                    {
                        {"Authorization", "Bearer " + oauth.access_token }
                    }
                };



                //--------------------------------------------------------------- callback url ---------------------------------------------------------------------
                string callbackUrl = string.Format("{0}/api/forge/designautomation/workitems/configuremodel/callback?connectionId={1}&modelBucketKey={2}&modelObjectKey={3}&pathInZip={4}",
                                                    Utils.GetAppSetting("FORGE_CALLBACK_URL"),
                                                    inputs.connectionId,
                                                    inputs.modelBucketKey,
                                                    outputModelName,
                                                    inputs.pathInZip);

                XrefTreeArgument callbackUrlArgument = new XrefTreeArgument()
                {
                    Verb = Verb.Post,
                    Url = callbackUrl
                };



                //--------------------------------------------------------------- submit workitem ---------------------------------------------------------------------
                WorkItem workItemSpec = new WorkItem()
                {
                    ActivityId = inputs.activityId,
                    Arguments = new Dictionary<string, IArgument>()
                    {
                        { "inputModel", inputModelArgument },
                        { "inventorParams", inventorParamsArgument },
                        { "outputModel", outputModelArgument },
                        { "outputPdf", outputPdfArgument },
                        { "outputStp", outputStpArgument },
                        { "onComplete", callbackUrlArgument }
                    }
                };

                // submit workitem
                WorkItemStatus workItemStatus = await _designAutomation.CreateWorkItemAsync(workItemSpec);

                return Ok(new { Result = "Workitem started" });
            }
            catch (Exception ex)
            {
                await _hubContext.Clients.Client(inputs.connectionId).SendAsync("onError", ex.ToString());
                return BadRequest(new { Error = ex.ToString() });
            }

        }


        /// <summary>
        /// Callback from Design Automation Workitem (onComplete) 
        /// </summary>
        [HttpPost]
        [Route("api/forge/designautomation/workitems/configuremodel/callback")]
        public async Task<IActionResult> Workitem_OnCallback(string connectionId, string modelBucketKey, string modelObjectKey, string pathInZip, [FromBody] dynamic body)
        {
            try
            {
                //parse json data
                JObject workItem = JObject.Parse((string)body.ToString());
                string status = workItem["status"].Value<string>();
                string reportUrl = workItem["reportUrl"].Value<string>();

                //download workitem report
                RestClient client = new RestClient(reportUrl);
                RestRequest request = new RestRequest(string.Empty);
                string reportTxt = Encoding.Default.GetString(client.DownloadData(request));

                await _hubContext.Clients.Client(connectionId).SendAsync("workItemComplete", workItem.ToString());
                await _hubContext.Clients.Client(connectionId).SendAsync("onProgress", reportTxt);

                if (status == "success")
                {
                    //start translation if job was successful
                    HttpResponseMessage requestTranslation = await ModelDerivative.TranslateObject(connectionId, modelBucketKey, modelObjectKey, pathInZip);
                    await _hubContext.Clients.Client(connectionId).SendAsync("translationRequested", "Translation requested: " + requestTranslation.ToString());
                }
            }
            catch (Exception ex)
            {
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }

        #endregion


    }


}

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


namespace CadflairWebApplication.Controllers.Forge
{
    /// <summary>
    /// Class used by SignalR
    /// </summary>
    public class DesignAutomationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public string GetConnectionId() { return Context.ConnectionId; }
    }

    [ApiController]
    public class DesignAutomationController : ControllerBase
    {
        #region Fields

        // Used to access the application folder (temp location for files & bundles)
        private IWebHostEnvironment _env;

        // used to access the SignalR Hub
        private IHubContext<DesignAutomationHub> _hubContext;

        // Design Automation v3 API
        DesignAutomationClient _designAutomation;


        #endregion

        #region Methods

        // Constructor, where env and hubContext are specified
        public DesignAutomationController(IWebHostEnvironment env, IHubContext<DesignAutomationHub> hubContext, DesignAutomationClient api)
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
                JObject payload = bodyJson["payload"].Value<JObject>();
                JObject workflowAttributes = payload["WorkflowAttributes"].Value<JObject>();

                string resourceURN = bodyJson["resourceUrn"].Value<string>();
                string id = workflowAttributes["connectionId"].Value<string>();

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
                dynamic oauth = OAuthController.GetInternal();

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

                await OSSController.CreateBucket(inputs.modelBucketKey);
                await OSSController.CreateBucket(inputs.pdfBucketKey);
                await OSSController.CreateBucket(inputs.stpBucketKey);

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
                    HttpResponseMessage requestTranslation = await ModelDerivativeController.TranslateObject(connectionId, modelBucketKey, modelObjectKey, pathInZip);
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


//DELETE THE CODE BELOW



//Extra stuff from the tutorial

///// Prefix for AppBundles and Activities
//public static string NickName { get { return OAuthController.GetAppSetting("FORGE_CLIENT_ID"); } }
///// Alias for the app (e.g. DEV, STG, PROD). This value may come from an environment variable
//public static string Alias { get { return "dev"; } }

/// <summary>
/// Get the status of a workitem.
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
//[HttpGet]
//[Route("api/forge/designautomation/workitems/workitemid")]
//public async Task<IActionResult> GetWorkItemStatus([FromQuery] string id)//StatusHeader statusHeader)
//{
//    WorkItemStatus workItemStatus = await _designAutomation.GetWorkitemStatusAsync(id);//statusHeader.ID);

//    if (workItemStatus.Status.ToString() != "Success")
//    {
//        return Ok(new { Report = "Status of workitem " + workItemStatus.Id + ": " + workItemStatus.Status.ToString() });
//    }

//    var client = new RestClient(workItemStatus.ReportUrl);
//    var request = new RestRequest(string.Empty);

//    byte[] bs = client.DownloadData(request);
//    string report = System.Text.Encoding.Default.GetString(bs);

//    return Ok(new { Report = report });
//}











        ///// <summary>
        ///// Input for StartWorkitem
        ///// </summary>
        //public class WorkItemInputs
        //{
        //    public string inventorParams { get; set; }
        //    public string connectionId { get; set; }
        //}






        ///// <summary>
        ///// Start a new workitem to create a dresser model
        ///// </summary>
        //[HttpPost]
        //[Route("api/forge/designautomation/workitems/createdressermodel")]
        //public async Task<IActionResult> CreateDresserModel([FromForm] WorkItemInputs inputs)
        //{
        //    try
        //    {
        //        // pull out input parameters and generate part number
        //        JObject formInputData = JObject.Parse(inputs.inventorParams);
        //        double widthParam = formInputData["dresserWidth"].Value<double>();
        //        double heigthParam = formInputData["dresserHeight"].Value<double>();
        //        double depthParam = formInputData["dresserDepth"].Value<double>();
        //        double legParam = formInputData["legExposure"].Value<double>();
        //        string rowsParam = formInputData["drawerRows"].Value<string>();
        //        string columnsParam = formInputData["drawerColumns"].Value<string>();
        //        string finishParam = formInputData["finishStyle"].Value<string>();
        //        string edgeParam = formInputData["edgeStyle"].Value<string>();
        //        string handleParam = formInputData["handleStyle"].Value<string>();

        //        string fnsh;
        //        switch (finishParam)
        //        {
        //            case "Light Birch":
        //                fnsh = "b";
        //                break;
        //            case "Matte Gray":
        //                fnsh = "g";
        //                break;
        //            case "Matte White":
        //                fnsh = "w";
        //                break;
        //            case "Stained Oak":
        //                fnsh = "o";
        //                break;
        //            default:
        //                fnsh = "";
        //                break;
        //        }

        //        string hdl;
        //        switch (handleParam)
        //        {
        //            case "Round Knob":
        //                hdl = "k";
        //                break;
        //            case "Bin Pull":
        //                hdl = "p";
        //                break;
        //            case "Square Handle":
        //                hdl = "h";
        //                break;
        //            default:
        //                hdl = "";
        //                break;
        //        }

        //        string edg;
        //        switch (edgeParam)
        //        {
        //            case "Square":
        //                edg = "s";
        //                break;
        //            case "Round":
        //                edg = "r";
        //                break;
        //            case "Elegant":
        //                edg = "e";
        //                break;
        //            default:
        //                edg = "";
        //                break;
        //        }

        //        string partNumber = "h" + heigthParam.ToString() + "w" + widthParam.ToString() + "d" + depthParam.ToString() + "-r" + rowsParam + "c" + columnsParam + "l" + legParam.ToString() + "-" + fnsh + hdl + edg;


        //        // OAuth token
        //        dynamic oauth = await OAuthController.GetInternalAsync();

        //        //input file arguments
        //        string inputbucketKey = "dresserbasemodel";
        //        string inputObjectID = "Dresser.zip";
        //        string pathInZip = "Dresser Assembly.iam";

        //        // 1. input file
        //        XrefTreeArgument inputFileArgument = new XrefTreeArgument()
        //        {
        //            Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputbucketKey, inputObjectID),
        //            PathInZip = pathInZip,
        //            Verb = Verb.Get,
        //            Headers = new Dictionary<string, string>()
        //            {
        //                { "Authorization", "Bearer " + oauth.access_token }
        //            }
        //        };

        //        // 2. input json
        //        dynamic inputJson = new JObject();
        //        inputJson.action = "GenerateModel";
        //        inputJson.partNumber = partNumber.ToUpper();
        //        inputJson.dresserWidth = widthParam * 2.54;
        //        inputJson.dresserHeight = heigthParam * 2.54;
        //        inputJson.dresserDepth = depthParam * 2.54;
        //        inputJson.legExposure = legParam * 2.54;
        //        inputJson.drawerRows = rowsParam;
        //        inputJson.drawerColumns = columnsParam;
        //        inputJson.finishStyle = finishParam;
        //        inputJson.edgeStyle = edgeParam;
        //        inputJson.handleStyle = handleParam;
        //        inputJson.legDepth = 3 * 2.54;
        //        inputJson.legWidth = 3 * 2.54;

        //        XrefTreeArgument inputJsonArgument = new XrefTreeArgument()
        //        {
        //            Url = "data:application/json, " + ((JObject)inputJson).ToString(Formatting.None).Replace("\"", "'")
        //        };

        //        //output file arguments
        //        string outputBucketKey = partNumber;
        //        string outputIamName = partNumber.ToUpper() + " Model.zip";  //"Dresser Assembly.zip"; 
        //        string outputPdfName = partNumber.ToUpper() + " Appr.pdf"; //"Dresser Drawing.pdf";

        //        //make new bucket
        //        BucketsApi buckets = new BucketsApi();
        //        buckets.Configuration.AccessToken = oauth.access_token;
        //        try
        //        {
        //            PostBucketsPayload bucketPayload = new PostBucketsPayload(outputBucketKey, null, PostBucketsPayload.PolicyKeyEnum.Temporary);
        //            await buckets.CreateBucketAsync(bucketPayload, "US");
        //        }
        //        catch
        //        {
        //            // in case bucket already exists
        //            dynamic resultObjectDetails = await OSSController.GetObjectDetailsAsync(outputBucketKey, outputIamName);
        //            return Ok(new { Result = "Configuration already exists", OutputBucketKey = outputBucketKey, Urn = (string)resultObjectDetails.encodedURN }); ;
        //        };

        //        // 3. output files     
        //        XrefTreeArgument outputIamArgument = new XrefTreeArgument()
        //        {
        //            Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", outputBucketKey, outputIamName),
        //            Verb = Verb.Put,
        //            Headers = new Dictionary<string, string>()
        //            {
        //                {"Authorization", "Bearer " + oauth.access_token }
        //            }
        //        };

        //        XrefTreeArgument outputPdfArgument = new XrefTreeArgument()
        //        {
        //            Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", outputBucketKey, outputPdfName),
        //            Verb = Verb.Put,
        //            Headers = new Dictionary<string, string>()
        //            {
        //                {"Authorization", "Bearer " + oauth.access_token }
        //            }
        //        };

        //        // prepare workitem
        //        string callbackUrl = string.Format("{0}/api/forge/designautomation/workitems/callback?id={1}&outputBucketKey={2}&outputIamName={3}&rootFileName={4}&outputPdfName={5}", Utils.GetAppSetting("FORGE_CALLBACK_URL"), inputs.connectionId, outputBucketKey, outputIamName, pathInZip, outputPdfName);
        //        WorkItem workItemSpec = new WorkItem()
        //        {
        //            ActivityId = "jgaukler.CreateDresserModel+v1",
        //            Arguments = new Dictionary<string, IArgument>()
        //            {
        //                { "InputFile", inputFileArgument },
        //                { "InventorParams",  inputJsonArgument },
        //                { "OutputIam", outputIamArgument },
        //                { "OutputPdf", outputPdfArgument },
        //                { "onComplete", new XrefTreeArgument { Verb = Verb.Post, Url = callbackUrl } }
        //            }
        //        };

        //        // submit workitem
        //        WorkItemStatus workItemStatus = await _designAutomation.CreateWorkItemAsync(workItemSpec);

        //        return Ok(new { Result = "Workitem started", WorkItemId = workItemStatus.Id, OutputBucketKey = outputBucketKey });
        //    }
        //    catch (Exception ex)
        //    {
        //        //await _hubContext.Clients.All.SendAsync("onComplete", "error: " + e.ToString());
        //        return BadRequest(ex.ToString());
        //    }

        //}

   ///// <summary>
   //     /// Callback from Design Automation Workitem (onProgress or onComplete) 
   //     /// </summary>
   //     [HttpPost]
   //     [Route("api/forge/designautomation/workitems/callback")]
   //     public async Task<IActionResult> Workitem_OnCallback(string id, string outputBucketKey, string outputIamName, string rootFileName, string outputPdfName, [FromBody] dynamic body)
   //     {
   //         try
   //         {
   //             //parse json data
   //             JObject workItem = JObject.Parse((string)body.ToString());
   //             await _hubContext.Clients.Client(id).SendAsync("workItemComplete", workItem.ToString());

   //             //download workitem report
   //             RestClient client = new RestClient(workItem["reportUrl"].Value<string>());
   //             string reportTxt = Encoding.Default.GetString(client.DownloadData(new RestRequest(string.Empty)));
   //             await _hubContext.Clients.Client(id).SendAsync("onProgress", reportTxt);

   //             //create download links for result files
   //             dynamic signedIamUrl = await OSSController.ObjectsApi.CreateSignedResourceAsyncWithHttpInfo(outputBucketKey, outputIamName, new PostBucketsSigned(10), "read");
   //             dynamic signedPdfUrl = await OSSController.ObjectsApi.CreateSignedResourceAsyncWithHttpInfo(outputBucketKey, outputPdfName, new PostBucketsSigned(10), "read");

   //             await _hubContext.Clients.Client(id).SendAsync("downloadResult", "pdf", (string)signedPdfUrl.Data.signedUrl);
   //             await _hubContext.Clients.Client(id).SendAsync("downloadResult", "zip", (string)signedIamUrl.Data.signedUrl);

   //             //start translation if job was successful
   //             if (workItem["status"].Value<string>() == "success")
   //             {
   //                 //get the urn and start the translation
   //                 dynamic resultObjectDetails = await OSSController.GetObjectDetailsAsync(outputBucketKey, outputIamName);
   //                 HttpResponseMessage requestTranslation = await ModelDerivativeController.TranslateObject(id, (string)resultObjectDetails.encodedURN, rootFileName);
   //                 await _hubContext.Clients.Client(id).SendAsync("translationRequested", "Translation requested: " + requestTranslation.ToString());

   //                 //dynamic translationJob = await ModelDerivativeController.TranslateObject((string)resultObjectDetails.encodedURN, rootFileName);
   //                 //JObject jobJson = JObject.Parse((string)translationJob.ToString());
   //                 //await _hubContext.Clients.Client(id).SendAsync("onComplete", "job JSON: " + jobJson.ToString());                  
   //             }

   //         }
   //         catch (Exception ex)
   //         {
   //         }

   //         //ALWAYS RETURN OK
   //         return Ok();
   //     }





///// <summary>
//        /// Start a new workitem to create a dresser model
//        /// </summary>
//        [HttpPost]
//        [Route("api/forge/designautomation/workitems/createsilomodel")]
//        public async Task<IActionResult> CreateSiloModel([FromForm] WorkItemInputs inputs)
//        {
//            try
//            {
//                // pull out input parameters
//                JObject formInputData = JObject.Parse(inputs.inventorParams);
//                string innerDiam = formInputData["innerDiam"].Value<string>();
//                string siloHeight = formInputData["siloHeight"].Value<string>();
//                string coneAngle = formInputData["coneAngle"].Value<string>();
//                string outletDiam = formInputData["outletDiam"].Value<string>();
//                string dischargeHeight = formInputData["dischargeHeight"].Value<string>();
//                string ladderAngle = formInputData["ladderAngle"].Value<string>();

//                string partNumber = "d" + innerDiam + "h" + siloHeight + "c" + coneAngle + "-o" + outletDiam + "d" + dischargeHeight + "l" + ladderAngle;

//                // OAuth token
//                dynamic oauth = OAuthController.GetInternal();

//                //input file arguments
//                string inputbucketKey = "siloconfiguratormodel";
//                string inputObjectID = "Silo.ipt";

//                // 1. input file
//                XrefTreeArgument inputFileArgument = new XrefTreeArgument()
//                {
//                    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputbucketKey, inputObjectID),
//                    Verb = Verb.Get,
//                    Headers = new Dictionary<string, string>()
//                    {
//                        { "Authorization", "Bearer " + oauth.access_token }
//                    }
//                };

//                // 2. input json
//                dynamic inputJson = new JObject();
//                inputJson.partNumber = partNumber.ToUpper();
//                inputJson.innerDiam = innerDiam;
//                inputJson.siloHeight = siloHeight;
//                inputJson.coneAngle = coneAngle;
//                inputJson.outletDiam = outletDiam;
//                inputJson.dischargeHeight = dischargeHeight;
//                inputJson.ladderAngle = ladderAngle;

//                XrefTreeArgument inputJsonArgument = new XrefTreeArgument()
//                {
//                    Url = "data:application/json, " + ((JObject)inputJson).ToString(Formatting.None).Replace("\"", "'")
//                };

//                //output file arguments
//                string outputBucketKey = Guid.NewGuid().ToString();
//                string outputIptName = partNumber.ToUpper() + ".ipt";
//                string outputPdfName = partNumber.ToUpper() + " Appr.pdf";

//                //make new bucket for outputs
//                BucketsApi buckets = new BucketsApi();
//                buckets.Configuration.AccessToken = oauth.access_token;
//                try
//                {
//                    //part number has spaces in it which is causing bucket creation to fail. Also need to check the activity on Postman
//                    PostBucketsPayload bucketPayload = new PostBucketsPayload(outputBucketKey, null, PostBucketsPayload.PolicyKeyEnum.Temporary);
//                    await buckets.CreateBucketAsync(bucketPayload, "US");
//                }
//                catch
//                {
//                    // in case bucket already exists do not recreate this configuration
//                    dynamic resultObjectDetails = await OSSController.GetObjectDetailsAsync(outputBucketKey, outputIptName);
//                    return Ok(new { Result = "Configuration already exists", OutputBucketKey = outputBucketKey, Urn = (string)resultObjectDetails.encodedURN }); ;
//                };

//                // 3. output files     
//                XrefTreeArgument outputIptArgument = new XrefTreeArgument()
//                {
//                    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", outputBucketKey, outputIptName),
//                    Verb = Verb.Put,
//                    Headers = new Dictionary<string, string>()
//                    {
//                        {"Authorization", "Bearer " + oauth.access_token }
//                    }
//                };

//                //XrefTreeArgument outputPdfArgument = new XrefTreeArgument()
//                //{
//                //    Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", outputBucketKey, outputPdfName),
//                //    Verb = Verb.Put,
//                //    Headers = new Dictionary<string, string>()
//                //    {
//                //        {"Authorization", "Bearer " + oauth.access_token }
//                //    }
//                //};

//                // prepare workitem
//                string callbackUrl = string.Format("{0}/api/forge/callback/createsilomodel?id={1}&outputBucketKey={2}&outputIptName={3}&outputPdfName={4}", Utils.GetAppSetting("FORGE_CALLBACK_URL"), inputs.connectionId, outputBucketKey, outputIptName, outputPdfName);
//                WorkItem workItemSpec = new WorkItem()
//                {
//                    ActivityId = "jgaukler.ConfigureSiloModel+v1",
//                    Arguments = new Dictionary<string, IArgument>()
//                    {
//                        { "InputIpt", inputFileArgument },
//                        { "InventorParams",  inputJsonArgument },
//                        { "OutputIpt", outputIptArgument },
//                        { "onComplete", new XrefTreeArgument { Verb = Verb.Post, Url = callbackUrl } }
//                    }
//                };

//                //{ "OutputPdf", outputPdfArgument },

//                // submit workitem
//                WorkItemStatus workItemStatus = await _designAutomation.CreateWorkItemAsync(workItemSpec);

//                return Ok(new { Result = "Workitem started", WorkItemId = workItemStatus.Id, OutputBucketKey = outputBucketKey });
//            }
//            catch (Exception e)
//            {
//                await _hubContext.Clients.All.SendAsync("onError", "error: " + e.ToString());
//                return BadRequest();
//            }

//        }

//        /// <summary>
//        /// Callback from Design Automation Workitem (onProgress or onComplete) for creating the 3D dresser model
//        /// </summary>
//        [HttpPost]
//        [Route("/api/forge/callback/createsilomodel")]
//        public async Task<IActionResult> CreateSiloModel_OnCallback(string id, string outputBucketKey, string outputIptName, string outputPdfName, [FromBody] dynamic body)
//        {

//            try
//            {
//                //parse json data
//                JObject workItem = JObject.Parse((string)body.ToString());
//                await _hubContext.Clients.Client(id).SendAsync("workItemComplete", workItem.ToString());

//                //download workitem report
//                RestClient client = new RestClient(workItem["reportUrl"].Value<string>());
//                string reportTxt = Encoding.Default.GetString(client.DownloadData(new RestRequest(string.Empty)));
//                await _hubContext.Clients.Client(id).SendAsync("onProgress", reportTxt);

//                //create download links for result files
//                //ObjectsApi objectsApi = new ObjectsApi();
//                //dynamic signedIptUrl = await objectsApi.CreateSignedResourceAsyncWithHttpInfo(outputBucketKey, outputIptName, new PostBucketsSigned(10), "read");
//                //dynamic signedPdfUrl = await objectsApi.CreateSignedResourceAsyncWithHttpInfo(outputBucketKey, outputPdfName, new PostBucketsSigned(10), "read");
//                //await _hubContext.Clients.Client(id).SendAsync("downloadResult", "pdf", (string)signedPdfUrl.Data.signedUrl);
//                //await _hubContext.Clients.Client(id).SendAsync("downloadResult", "zip", (string)signedIptUrl.Data.signedUrl);

//                //start translation if job was successful
//                if (workItem["status"].Value<string>() == "success")
//                {
//                    //get the urn and start the translation
//                    HttpResponseMessage requestTranslation = await ModelDerivativeController.TranslateObject(id, outputBucketKey, outputIptName);
//                    await _hubContext.Clients.Client(id).SendAsync("translationRequested", "Translation requested: " + requestTranslation.ToString());
//                }

//            }
//            catch (Exception ex)
//            {
//                await _hubContext.Clients.Client(id).SendAsync("onError", "error: " + ex.ToString());
//            }

//            // ALWAYS return ok (200)
//            return Ok();
//        }


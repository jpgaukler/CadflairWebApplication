using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CadflairBlazorServer.Controllers
{

    /// <summary>
    /// Signal R hub for sending messages back to the client after responses are received from Forge.
    /// </summary>
    public class ForgeCallbackHub : Hub
    {
        public string GetConnectionId() { return Context.ConnectionId; }
    }


    [ApiController]
    public class ForgeCallbackController : ControllerBase
    {
        private readonly ForgeServicesManager _forgeServicesManager;
        private readonly DataServicesManager _dataServicesManager;
        private readonly IHubContext<ForgeCallbackHub> _hubContext;
        private readonly ILogger<ForgeCallbackController> _logger;

        public ForgeCallbackController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager, IHubContext<ForgeCallbackHub> hubContext, ILogger<ForgeCallbackController> logger)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
            _hubContext = hubContext;
            _logger = logger;
        }


        [HttpPost]
        [Route("api/v1/designautomation/productconfiguration/create/oncomplete/{connectionId}/{productConfigurationId}/{outputBucketKey}/{outputZipKey}/{outputStpKey}")]
        public async Task<IActionResult> CreateProductConfigurationModel_OnComplete(string connectionId, int productConfigurationId, string outputBucketKey, string outputZipKey, string outputStpKey, [FromBody] dynamic body)
        {
            try
            {
                // parse json
                JObject response = JObject.Parse((string)body.ToString());
                Debug.WriteLine($"Workitem complete: {response}");
                string status = (string)response["status"]!;
                string reportUrl = (string)response["reportUrl"]!;

                if (status == "success")
                {
                    // update database record
                    ProductConfiguration productConfiguration = await _dataServicesManager.ProductService.GetProductConfigurationById(productConfigurationId);
                    productConfiguration.BucketKey = outputBucketKey;
                    productConfiguration.ZipObjectKey = outputZipKey;
                    productConfiguration.StpObjectKey = outputStpKey;
                    await _dataServicesManager.ProductService.UpdateProductConfiguration(productConfiguration);

                    // send message to client
                    await _hubContext.Clients.Client(connectionId).SendAsync("CreateProductConfigurationModel_OnComplete", productConfigurationId);
                }
                else
                {
                    // download workitem report
                    RestClient client = new(reportUrl);
                    byte[]? bytes = await client.DownloadDataAsync(new RestRequest());
                    string reportTxt = System.Text.Encoding.Default.GetString(bytes!);

                    _logger.LogError($"Workitem failed: {reportTxt}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }


        [HttpPost]
        [Route("api/v1/designautomation/productconfiguration/create/onprogress/{connectionId}")]
        public async Task<IActionResult> CreateProductConfigurationModel_OnProgress(string connectionId, [FromBody] dynamic body)
        {
            try
            {
                JObject response = JObject.Parse((string)body.ToString());

                if (response["progress"] != null)
                {
                    JObject progress = JObject.Parse((string)response["progress"]!);
                    string message = (string)progress["message"]!;

                    // send message to client
                    await _hubContext.Clients.Client(connectionId).SendAsync("CreateProductConfigurationModel_OnProgress", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }


        /// <summary>
        /// Return a list of signed urls (one for each file name) to the design automation service so all svf files can be uploaded directly to Forge OSS. 
        /// <br/><br/>
        /// See blog post here: <see href="https://aps.autodesk.com/blog/speed-viewable-generation-when-using-design-automation-inventor">Speed up viewable generation when using Design Automation for Inventor</see>
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/designautomation/productconfiguration/create/onviewables/{bucketKey}")]
        public async Task<IActionResult> CreateProductConfigurationModel_OnViewables(string bucketKey, [FromBody] dynamic body)
        {
            try
            {
                // create signed upload urls for the given bucket key, to upload the resulting svf files to OSS
                JObject response = JObject.Parse((string)body.ToString());
                string permissions = (string)response["permissions"]!;
                JArray files = (JArray)response["files"]!;

                Dictionary<string, string> signedUrls = new();

                foreach (var filename in files)
                {
                    string objectKey = (string)filename!;
                    string signedUrl = await _forgeServicesManager.ObjectStorageService.GetSignedUploadUrl(bucketKey, objectKey, minuteExpiration: 15, singleUse: true);
                    signedUrls.Add(objectKey, signedUrl);
                }

                // return the list of signed urls as a json array
                return Ok(JsonConvert.SerializeObject(signedUrls));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }


        [HttpPost]
        [Route("/api/v1/modelderivative/translate/oncomplete")]
        public async Task<IActionResult> ModelDerivativeTranslation_OnComplete([FromBody] dynamic body)
        {
            try
            {
                JObject response = JObject.Parse((string)body.ToString());
                JObject payload = (JObject)response["payload"]!;
                JObject workflowAttributes = (JObject)payload["WorkflowAttributes"]!;
                string resourceUrn = (string)response["resourceUrn"]!;
                string connectionId = (string)workflowAttributes["connectionId"]!;

                // send message to client
                await _hubContext.Clients.Client(connectionId).SendAsync("ModelDerivativeTranslation_OnComplete", resourceUrn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }


        /// <summary>
        /// Redirect a request from the Forge Viewer to load a model directly from a file is OSS. This method generates a signed url for the object and redirects the viewer to read from that location.
        /// <br/><br/>
        /// See blog post here: <see href="https://aps.autodesk.com/blog/speed-viewable-generation-when-using-design-automation-inventor">Speed up viewable generation when using Design Automation for Inventor</see>
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/v1/viewer_proxy/{bucketKey}/{*objectKey}")]
        public async Task<IActionResult> RedirectViewerToOSS(string bucketKey, string objectKey)
        {
            try
            {
                // must encode object key as URL form
                string encodedObjectId = objectKey.Replace("/", "%2F");
                string redirectUri = await _forgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(bucketKey, encodedObjectId);
                return Redirect(redirectUri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}

using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;

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

        public ForgeCallbackController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager, IHubContext<ForgeCallbackHub> hubContext)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
            _hubContext = hubContext;
        }


        [HttpPost]
        [Route("api/forge/designautomation/productconfiguration/create/oncomplete")]
        public async Task<IActionResult> CreateProductConfiguration_OnComplete(string connectionId, int productConfigurationId, string outputBucketKey, string outputObjectKey, string rootFileName, [FromBody] dynamic body)
        {
            try
            {
                // parse json
                JObject response = JObject.Parse((string)body.ToString());
                string status = response["status"]?.Value<string>()!;
                string reportUrl = response["reportUrl"]?.Value<string>()!;

                if (status == "success")
                {
                    // translate result
                    await _forgeServicesManager.ModelDerivativeService.TranslateObject(outputBucketKey, outputObjectKey, rootFileName, connectionId);

                    // update database record
                    ProductConfiguration productConfiguration = await _dataServicesManager.ProductService.GetProductConfigurationById(productConfigurationId);
                    productConfiguration.ForgeZipKey = outputObjectKey;
                    await _dataServicesManager.ProductService.UpdateProductConfiguration(productConfiguration);

                    // send message to client
                    await _hubContext.Clients.Client(connectionId).SendAsync("CreateProductConfiguration_OnComplete", productConfigurationId);
                }
                else
                {
                    // download workitem report
                    RestClient client = new(reportUrl);
                    byte[]? bytes = await client.DownloadDataAsync(new RestRequest());
                    string reportTxt = System.Text.Encoding.Default.GetString(bytes!);

                    Debug.WriteLine($"Workitem failed: {reportTxt}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }

        [HttpPost]
        [Route("api/forge/designautomation/productconfiguration/create/onprogress")]
        public async Task<IActionResult> CreateProductConfiguration_OnProgress(string connectionId, [FromBody] dynamic body)
        {
            try
            {
                JObject response = JObject.Parse((string)body.ToString());

                if (response["progress"] != null)
                {
                    JObject progress = JObject.Parse(response["progress"]?.Value<string>()!);
                    string message = progress["message"]?.Value<string>()!;

                    // send message to client
                    await _hubContext.Clients.Client(connectionId).SendAsync("CreateProductConfiguration_OnProgress", message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }

        [HttpPost]
        [Route("/api/forge/modelderivative/translate/oncomplete")]
        public async Task<IActionResult> ModelDerivativeTranslation_OnComplete([FromBody] dynamic body)
        {
            try
            {
                Debug.WriteLine("Model Derivative translation OnComplete");
                JObject response = JObject.Parse((string)body.ToString());
                JObject payload = response["payload"]?.Value<JObject>()!;
                JObject workflowAttributes = payload["WorkflowAttributes"]?.Value<JObject>()!;
                string resourceUrn = response["resourceUrn"]?.Value<string>()!;
                string connectionId = workflowAttributes["connectionId"]?.Value<string>()!;

                // send message to client
                await _hubContext.Clients.Client(connectionId).SendAsync("ModelDerivativeTranslation_OnComplete", resourceUrn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            // ALWAYS return ok (200)
            return Ok();
        }


    }
}

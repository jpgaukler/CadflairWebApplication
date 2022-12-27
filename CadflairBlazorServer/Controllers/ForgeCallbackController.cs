using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;

namespace CadflairBlazorServer.Controllers
{
    [ApiController]
    public class ForgeCallbackController : ControllerBase
    {

        private readonly ForgeServicesManager _forgeServicesManager;
        private readonly DataServicesManager _dataServicesManager;

        public ForgeCallbackController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
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

                // download workitem report
                RestClient client = new(reportUrl);
                byte[]? bytes = await client.DownloadDataAsync(new RestRequest());
                string reportTxt = System.Text.Encoding.Default.GetString(bytes!);

                Debug.WriteLine($"Workitem complete: {reportTxt}");

                if (status == "success")
                {
                    // update database record
                    ProductConfiguration productConfiguration = await _dataServicesManager.ProductService.GetProductConfigurationById(productConfigurationId);
                    productConfiguration.ForgeZipKey = Guid.Parse(outputObjectKey);
                    await _dataServicesManager.ProductService.UpdateProductConfiguration(productConfiguration);

                    // translate result
                    var tranlationJob = await _forgeServicesManager.ModelDerivativeService.TranslateObject(outputBucketKey, outputObjectKey, rootFileName);


                    //await _hubContext.Clients.Client(connectionId).SendAsync("translationRequested", "Translation requested: " + requestTranslation.ToString());
                    //await _hubContext.Clients.Client(connectionId).SendAsync("workItemComplete", workItem.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }

        [HttpPost]
        [Route("api/forge/designautomation/productconfiguration/create/onprogress")]
        public async Task<IActionResult> CreateProductConfiguration_OnProgress([FromBody] dynamic body)
        {
            try
            {
                JObject response = JObject.Parse((string)body.ToString());

                if (response["progress"] != null)
                {
                    JObject progress = JObject.Parse(response["progress"]?.Value<string>()!);
                    string message = progress["message"]?.Value<string>()!;

                    Debug.WriteLine($"WorkItem progress: {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }

            //ALWAYS RETURN OK TO THE FORGE API
            return Ok();
        }


    }
}

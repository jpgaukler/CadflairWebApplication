using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CadflairBlazorServer.Controllers
{
    [ApiController]
    public class ForgeCallbackController : ControllerBase
    {

        public ForgeCallbackController()
        {
            //inject services here if they are required

        }

        ///// <summary>
        ///// Callback from Design Automation Workitem (onComplete) 
        ///// </summary>
        //[HttpPost]
        //[Route("api/forge/designautomation/productconfiguration/generate/callback")]
        //public async Task<IActionResult> GenerateProductConfiguration_OnCallback(string connectionId, string modelBucketKey, string modelObjectKey, string pathInZip, [FromBody] dynamic body)
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
        //            HttpResponseMessage requestTranslation = await ModelDerivativeController.TranslateObject(connectionId, modelBucketKey, modelObjectKey, pathInZip);
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

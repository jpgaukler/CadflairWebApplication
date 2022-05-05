using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forge.Controllers
{
    [ApiController]
    public class ModelDerivativeController : ControllerBase
    {

        //used to send http request for urn translation
        private static HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Start the translation job for a give bucketKey/objectName
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectURN"></param>
        /// <param name="rootFileName"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> TranslateObject(string id, string objectURN, string rootFileName = null)
        {
            dynamic oauth = await OAuthController.GetInternalAsync();

            dynamic translateJson = new
            {
                input = new
                {
                    urn = objectURN,
                    compressedUrn = rootFileName != null,
                    rootFilename = rootFileName
                },
                output = new
                {
                    destination = new
                    {
                        region = "us"
                    },
                    formats = new[]
                    {
                        new
                        {
                            type = "svf",
                            views = new[]
                            {
                                "2d", "3d"
                            }
                        }
                    },
                },
                misc = new
                {
                    workflow = "translateObjectWorkflow",
                    workflowAttribute = new
                    {
                        connectionId = id,
                    }
                }
            };

            var translationRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://developer.api.autodesk.com/modelderivative/v2/designdata/job"),
                Headers = {
                                { HttpRequestHeader.Authorization.ToString(), "Bearer " +  oauth.access_token},
                           },
                Content = new StringContent(JsonConvert.SerializeObject(translateJson), System.Text.Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = _httpClient.SendAsync(translationRequest).Result;
            return response;
        }

        ///// <summary>
        ///// Start the translation job for a give bucketKey/objectName (default method from tutorial project). This does not currently support workflow attributes, so the Signal R connection id can not be supplied
        ///// </summary>
        ///// <param name="connectionID"></param>
        ///// <param name="objectURN"></param>
        ///// <param name="rootFileName"></param>
        ///// <returns></returns>
        //public static async Task<dynamic> TranslateObject(string objectURN, string rootFileName)
        //{
        //    dynamic oauth = await OAuthController.GetInternalAsync();

        //    // prepare the payload
        //    List<JobPayloadItem> outputs = new List<JobPayloadItem>()
        //      {
        //       new JobPayloadItem(
        //         JobPayloadItem.TypeEnum.Svf,
        //         new List<JobPayloadItem.ViewsEnum>()
        //         {
        //           JobPayloadItem.ViewsEnum._2d,
        //           JobPayloadItem.ViewsEnum._3d
        //         })
        //      };

        //    JobPayload job;
        //    job = new JobPayload()
        //    {
        //        Input = new JobPayloadInput(objectURN, true, rootFileName),
        //        Output = new JobPayloadOutput(outputs),
        //        Misc = new JobPayloadMisc("translateObjectWorkflow")
        //    };

        //    //return job.ToJson();

        //    // start the translation
        //    DerivativesApi derivative = new DerivativesApi();
        //    derivative.Configuration.AccessToken = oauth.access_token;
        //    dynamic jobPosted = await derivative.TranslateAsync(job);
        //    return jobPosted;

        //}
    }
}

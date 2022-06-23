using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static async Task<HttpResponseMessage> TranslateObject(string _connectionId, string objectURN, string rootFileName = null)
        {
            dynamic oauth = await OAuthController.GetInternalAsync();

            dynamic translateJson = new
            {
                input = new
                {
                    urn = objectURN,
                    //compressedUrn = rootFileName != null,
                    //rootFilename = rootFileName ?? "",
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
                        connectionId = _connectionId,
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


        /// <summary>
        /// Translate an object in a given bucket
        /// </summary>
        /// <param name="_connectionId"></param>
        /// <param name="bucketKey"></param>
        /// <param name="objectKey"></param>
        /// <param name="rootFileName"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> TranslateObject(string _connectionId, string bucketKey, string objectKey, string rootFileName = null)
        {
            dynamic oauth = await OAuthController.GetInternalAsync();

            ObjectsApi objects = new ObjectsApi();
            objects.Configuration.AccessToken = oauth.access_token;

            string objectUrn = await OSSController.GetEncodedUrnAsync(bucketKey, objectKey);

            //input argument
            dynamic inputArg;
            if(rootFileName == null)
            {
                inputArg = new
                {
                    urn = objectUrn
                };
            }
            else
            {
                inputArg = new
                {
                    urn = objectUrn,
                    compressedUrn = true,
                    rootFilename = rootFileName
                };
            }

            //output argument
            dynamic outputArg = new
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
            };

            //workflow webhook
            dynamic miscArg = new
            {
                workflow = "translateObjectWorkflow",
                workflowAttribute = new
                {
                    connectionId = _connectionId,
                }
            };


            //json Request
            dynamic translateJson = new
            {
                input = inputArg,
                output = outputArg,
                misc = miscArg
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


        /// <summary>
        /// Get the thumbnail for a model in Forge OSS storage
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/forge/modelderivative/thumbnail")]
        public async Task<IActionResult> GetThumbnail(string urn)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                DerivativesApi derivative = new DerivativesApi();
                derivative.Configuration.AccessToken = oauth.access_token;

                Stream thumbnailStream = await derivative.GetThumbnailAsync(urn);
                byte[] byteArray;

                using (var memoryStream = new MemoryStream())
                {
                    thumbnailStream.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }

                return Ok(new { Base64String = Convert.ToBase64String(byteArray) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}

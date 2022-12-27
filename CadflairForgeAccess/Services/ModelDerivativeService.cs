using Autodesk.Forge;
using Autodesk.Forge.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CadflairForgeAccess.Services
{
    public class ModelDerivativeService
    {

        private readonly AuthorizationService _authService;
        private readonly ObjectStorageService _objectStorageService;

        //private readonly HttpClient _httpClient = new();

        public ModelDerivativeService(AuthorizationService authService, ObjectStorageService objectStorageService)
        {
            _authService = authService;
            _objectStorageService = objectStorageService;
        }

        private async Task<DerivativesApi> GetDerivativesApi()
        {
            DerivativesApi derivative = new();
            dynamic token = await _authService.GetInternal();
            derivative.Configuration.AccessToken = token.access_token;
            return derivative;
        }


        public async Task<dynamic> TranslateObject(string bucketKey, string objectKey, string rootFileName)
        {
            var objectDetails = await _objectStorageService.GetObjectDetails(bucketKey.ToString(), objectKey.ToString());

            // prepare the payload
            List<JobPayloadItem.ViewsEnum> views = new()
            {
                JobPayloadItem.ViewsEnum._2d,
                JobPayloadItem.ViewsEnum._3d
            };

            List<JobPayloadItem> payloadList = new()
            {
                new(JobPayloadItem.TypeEnum.Svf, views)
            };

            JobPayload job = new()
            {
                Input = new JobPayloadInput(objectDetails.encoded_urn, true, rootFileName),
                Output = new JobPayloadOutput(payloadList),
                Misc = new JobPayloadMisc("translateObjectWorkflow")
            };

            // start the translation
            DerivativesApi derivative = await GetDerivativesApi();
            dynamic jobPosted = await derivative.TranslateAsync(job);

            Debug.WriteLine($@"Translation started: {jobPosted}");
            return jobPosted;
        }

        public async Task<bool> TranslationExists(string encodedUrn)
        {
            try
            {
                DerivativesApi derivative = await GetDerivativesApi();
                dynamic manifest = await derivative.GetManifestAsync(encodedUrn);
                return true;
            }
            catch
            {
                return false;
            }
        }


        ///// <summary>
        ///// Translate an object in a given bucket
        ///// </summary>
        ///// <param name="_connectionId"></param>
        ///// <param name="bucketKey"></param>
        ///// <param name="objectKey"></param>
        ///// <param name="rootFileName"></param>
        ///// <returns></returns>
        //public async Task<HttpResponseMessage> TranslateObject(string _connectionId, string bucketKey, string objectKey, string? rootFileName = null)
        //{

        //    dynamic oauth = ForgeAuthorizationService.GetInternal();
        //    string? objectUrn = await ObjectStorageService.GetEncodedUrnAsync(bucketKey, objectKey);

        //    //input argument
        //    dynamic inputArg;
        //    if (rootFileName == null)
        //    {
        //        inputArg = new
        //        {
        //            urn = objectUrn
        //        };
        //    }
        //    else
        //    {
        //        inputArg = new
        //        {
        //            urn = objectUrn,
        //            compressedUrn = true,
        //            rootFilename = rootFileName
        //        };
        //    }

        //    //output argument
        //    dynamic outputArg = new
        //    {
        //        destination = new
        //        {
        //            region = "us"
        //        },
        //        formats = new[]
        //        {
        //                new
        //                {
        //                    type = "svf",
        //                    views = new[]
        //                    {
        //                        "2d", "3d"
        //                    }
        //                }
        //        },
        //    };

        //    //workflow webhook
        //    dynamic miscArg = new
        //    {
        //        workflow = "translateObjectWorkflow",
        //        workflowAttribute = new
        //        {
        //            connectionId = _connectionId,
        //        }
        //    };


        //    //json Request
        //    dynamic translateJson = new
        //    {
        //        input = inputArg,
        //        output = outputArg,
        //        misc = miscArg
        //    };

        //    var translationRequest = new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Post,
        //        RequestUri = new Uri("https://developer.api.autodesk.com/modelderivative/v2/designdata/job"),
        //        Headers =
        //        {
        //            { HttpRequestHeader.Authorization.ToString(), "Bearer " +  oauth.access_token},
        //        },
        //        Content = new StringContent(JsonConvert.SerializeObject(translateJson), System.Text.Encoding.UTF8, "application/json")
        //    };

        //    HttpResponseMessage response = await _httpClient.SendAsync(translationRequest);

        //    return response;
        //}



        ///// <summary>
        ///// Get the thumbnail for a model in Forge OSS storage
        ///// </summary>
        ///// <param name="urn"></param>
        ///// <returns></returns>
        //public async Task<string> GetThumbnailBase64String(string base64Urn)
        //{
        //    try
        //    {
        //        Stream thumbnailStream = await DerivativesApi.GetThumbnailAsync(base64Urn);

        //        byte[] byteArray;

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await thumbnailStream.CopyToAsync(memoryStream);
        //            byteArray = memoryStream.ToArray();
        //        }

        //        return Convert.ToBase64String(byteArray);
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }
        //}

    }
}

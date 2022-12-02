using Autodesk.Forge;
using Autodesk.Forge.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CadflairForgeAccess.Services
{
    public class ModelDerivativeService
    {

        private readonly DerivativesApi _derivativesApi = new();
        private readonly HttpClient _httpClient = new();

        public ModelDerivativeService()
        {

        }

        //internal DerivativesApi DerivativesApi
        //{
        //    get
        //    {
        //        dynamic oauth = ForgeAuthorizationService.GetInternal();
        //        _derivativesApi.Configuration.AccessToken = oauth.access_token;

        //        return _derivativesApi;
        //    }
        //}


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

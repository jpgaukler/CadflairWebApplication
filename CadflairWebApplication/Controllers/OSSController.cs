using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forge.Controllers
{
    [ApiController]
    public class OSSController : ControllerBase
    {
        ///// <summary>
        ///// Return list of buckets (id=#) or list of objects (id=bucketKey)
        ///// </summary>
        //[HttpGet]
        //[Route("api/forge/oss/buckets")]
        //public async Task<IList<TreeNode>> GetOSSAsync([FromQuery] string id)
        //{
        //    dynamic oauth = await OAuthController.GetInternalAsync();

        //    if (id == "#") // root
        //    {
        //        // in this case, let's return all buckets
        //        BucketsApi appBckets = new BucketsApi();
        //        appBckets.Configuration.AccessToken = oauth.access_token;

        //        // to simplify, let's return only the first 100 buckets
        //        dynamic buckets = await appBckets.GetBucketsAsync("US", 100);
        //        foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(buckets.items))
        //        {
        //            nodes.Add(new TreeNode(bucket.Value.bucketKey, bucket.Value.bucketKey.Replace(ClientId + "-", string.Empty), "bucket", true));
        //        }
        //    }
        //    else
        //    {
        //        // as we have the id (bucketKey), let's return all 
        //        ObjectsApi objects = new ObjectsApi();
        //        objects.Configuration.AccessToken = oauth.access_token;
        //        var objectsList = objects.GetObjects(id);
        //        foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
        //        {
        //            nodes.Add(new TreeNode(Base64Encode((string)objInfo.Value.objectId),
        //              objInfo.Value.objectKey, "object", false));
        //        }
        //    }
        //    return nodes;
        //}

        /// <summary>
        /// Return urn of an object within a bucket
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/buckets/bucket/object")]
        public async Task<IActionResult> GetObjectURNAsync([FromQuery] string bucketKey, [FromQuery] string objectName)
        {
            dynamic oauth = await OAuthController.GetInternalAsync();

            ObjectsApi objects = new ObjectsApi();
            objects.Configuration.AccessToken = oauth.access_token;
            try
            {
                dynamic objectDetails = await objects.GetObjectDetailsAsync(bucketKey, objectName);
                return Ok(new { Result = "Success", ObjectURN = Base64Encode((string)objectDetails.objectId) });
            }
            catch
            {
                return NotFound(new { Result = "Object Not Found" });
            }

        }

        /// <summary>
        /// Return urn of an object within a bucket
        /// </summary>
        public static async Task<dynamic> GetObjectDetailsAsync(string bucketKey, string objectName)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = oauth.access_token;

                dynamic objectDetails = await objects.GetObjectDetailsAsync(bucketKey, objectName);
                objectDetails.encodedURN = Base64Encode((string)objectDetails.objectId);
                return objectDetails;
            }
            catch
            {
                return null;
            }
           
        }

        /// <summary>
        /// Base64 enconde a string
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}

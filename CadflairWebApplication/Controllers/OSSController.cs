using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forge.Controllers
{
    [ApiController]
    public class OSSController : ControllerBase
    {
        /// <summary>
        /// Return list of buckets 
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/buckets")]
        public async Task<IActionResult> GetBucketsAsync(string startAt)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                // in this case, let's return all buckets
                BucketsApi appBuckets = new BucketsApi();
                appBuckets.Configuration.AccessToken = oauth.access_token;

                // to simplify, let's return only the first 100 buckets
                dynamic buckets = await appBuckets.GetBucketsAsync("US", 100);
                
                //format buckets into viewable format
                List<Bucket> bucketsList = new List<Bucket>();
                foreach (KeyValuePair<string, dynamic> item in new DynamicDictionaryItems(buckets.items))
                {
                    string bucketKey = item.Value.bucketKey;
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(item.Value.createdDate);
                    string createdDate = dateTimeOffset.Date.ToShortDateString(); 
                    string policyKey = item.Value.policyKey;

                    //do not return the base model bucket, so that it does not accidentally get deleted
                    if (bucketKey.Contains("basemodel")) continue;

                    Bucket bucket = new Bucket(bucketKey, null, createdDate, null, policyKey);
                    bucketsList.Add(bucket);
                }

                bucketsList.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));
                return Ok(new { Buckets = bucketsList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Delete a bucket from OSS
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/forge/oss/buckets/delete")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketKey)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                BucketsApi appBuckets = new BucketsApi();
                appBuckets.Configuration.AccessToken = oauth.access_token;
                appBuckets.DeleteBucket(bucketKey);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        ///// <summary>
        ///// Model data for bucket view used on GetBucketsASync
        ///// </summary>
        //public class BucketInfo
        //{
        //    public string bucketKey { get; set; }
        //    public long createdDate { get; set; }
        //}

        /// <summary>
        /// Return list of objects in a bucket
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/objects")]
        public async Task<IActionResult> GetObjectsAsync(string bucketKey)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                // as we have the id (bucketKey), let's return all 
                ObjectsApi objectsApi = new ObjectsApi();
                objectsApi.Configuration.AccessToken = oauth.access_token;
                dynamic objects = objectsApi.GetObjects(bucketKey);

                //format buckets into viewable format
                List<ObjectDetails> objectsList = new List<ObjectDetails>();
                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objects.items))
                {
                    string urn = Base64Encode((string)objInfo.Value.objectId);
                    string key = objInfo.Value.objectKey;

                    ObjectDetails objectDetails = new ObjectDetails(bucketKey, urn, key);
                    objectsList.Add(objectDetails);
                }

                return Ok(new { Objects = objectsList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

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

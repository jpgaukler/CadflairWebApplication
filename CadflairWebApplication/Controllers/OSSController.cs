using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Forge.Controllers
{
    [ApiController]
    public class OSSController : ControllerBase
    {
        public static async Task<dynamic> CreateBucket(string bucketKey)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                BucketsApi buckets = new BucketsApi();
                buckets.Configuration.AccessToken = oauth.access_token;

                PostBucketsPayload bucketPayload = new PostBucketsPayload(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Persistent);
                dynamic bucket = await buckets.CreateBucketAsync(bucketPayload, "US");

                return bucket;
            }
            catch
            {
                return null; //return null if the bucket can not be created, this can occur if the bucket already exists, or has an invalid name
            }
        }


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
                dynamic objects = objectsApi.GetObjects(bucketKey, 100);

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

        ///// <summary>
        ///// Return urn of an object within a bucket
        ///// </summary>
        //[HttpGet]
        //[Route("api/forge/oss/buckets/bucket/object")]
        //public async Task<IActionResult> GetObjectURNAsync([FromQuery] string bucketKey, [FromQuery] string objectName)
        //{
        //    dynamic oauth = await OAuthController.GetInternalAsync();

        //    ObjectsApi objects = new ObjectsApi();
        //    objects.Configuration.AccessToken = oauth.access_token;
        //    try
        //    {
        //        dynamic objectDetails = await objects.GetObjectDetailsAsync(bucketKey, objectName);
        //        return Ok(new { Result = "Success", ObjectURN = Base64Encode((string)objectDetails.objectId) });
        //    }
        //    catch
        //    {
        //        return NotFound(new { Result = "Object Not Found" });
        //    }

        //}

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

                //add the encoded urn to the object
                objectDetails.encodedURN = Base64Encode((string)objectDetails.objectId);

                return objectDetails;
            }
            catch
            {
                return null; //call to GetObjectDetailsAsync should fail if no object is found
            }

        }

        /// <summary>
        /// Return signed url to download an object
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/objects/getsignedurl")]
        public async Task<IActionResult> GetObjectSignedUrl(string bucketKey, string objectKey)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                ObjectsApi objectsApi = new ObjectsApi();
                objectsApi.Configuration.AccessToken = oauth.access_token;

                dynamic resource = await objectsApi.CreateSignedResourceAsync(bucketKey, objectKey, new PostBucketsSigned(10), "read");

                return Ok(new { Url = (string)resource.signedUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        /// <summary>
        /// Get the Base64 encoded URN for an object
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static async Task<string> GetEncodedUrnAsync(string bucketKey, string objectName)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = oauth.access_token;

                dynamic objectDetails = await objects.GetObjectDetailsAsync(bucketKey, objectName);
                return Base64Encode((string)objectDetails.objectId);
            }
            catch
            {
                return null;
            }

        }


        /// <summary>
        /// Deletes an object from OSS
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/forge/oss/objects/delete")]
        public async Task<IActionResult> DeleteObjectAsync(string bucketKey, string objectKey)
        {
            try
            {
                dynamic oauth = await OAuthController.GetInternalAsync();

                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = oauth.access_token;

                objects.DeleteObject(bucketKey, objectKey);

                return Ok(new { Result = objectKey + " successfully deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        public class ForgeFileUploadData
        {
            public string bucketKey { get; set; }
            public string objectName { get; set; }
            public IFormFile file { get; set; }
        }

        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/forge/oss/objects/upload")]
        public async Task<IActionResult> UploadObject([FromForm] ForgeFileUploadData fileUploadData)
        {
            try
            {
                string[] validFileTypes = { ".ipt", "iam", ".idw", ".dwg" };

                //validate data
                if (string.IsNullOrWhiteSpace(fileUploadData.bucketKey)) throw new Exception("bucketKey parameter was not provided.");
                if (string.IsNullOrWhiteSpace(fileUploadData.objectName)) throw new Exception("objectName parameter was not provided.");
                if (!validFileTypes.Any(i => i == Path.GetExtension(fileUploadData.file.FileName))) throw new Exception("Invalid file type provided.");
                if (fileUploadData.file.Length == 0) throw new Exception("File does not contain any data.");

                //// Upload check if less than 2mb!
                //if (memoryStream.Length < 2097152)
                //{
                //}
                //else
                //{
                //}

                // get the auth token
                dynamic oauth = await OAuthController.GetInternalAsync();
                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = oauth.access_token;

                using MemoryStream stream = new MemoryStream();
                await fileUploadData.file.CopyToAsync(stream);

                // upload the file/object, which will create a new object
                dynamic uploadedObj = await objects.UploadObjectAsync(bucketKey: fileUploadData.bucketKey,
                                                                      objectName: fileUploadData.objectName,
                                                                      contentLength: (int)stream.Length,
                                                                      body: stream);

                return Ok(new { Result = uploadedObj });
                //return Ok(new { BucketKey = fileUploadData.objectName, ObjectName = fileUploadData.objectName, FileLength = fileUploadData.file.Length.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Failed to upload object to Forge: {ex}" });
            }
        }

        /// <summary>
        /// Base64 enconde a string
        /// </summary>
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}

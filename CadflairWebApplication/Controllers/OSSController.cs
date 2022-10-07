using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CadflairWebApplication.Controllers.Forge
{
    [ApiController]
    public class OSSController : ControllerBase
    {

        #region Fields

        private static readonly ObjectsApi _objectsApi = new ObjectsApi();

        private static readonly BucketsApi _bucketsApi = new BucketsApi();

        private static readonly string[] _uploadFileExtensions = { ".ipt", "iam", ".idw", ".dwg", ".zip"};

        #endregion

        #region Properties

        internal static ObjectsApi ObjectsApi
        {
            get
            {
                dynamic oauth = OAuthController.GetInternal();
                _objectsApi.Configuration.AccessToken = oauth.access_token;

                return _objectsApi;
            }
        }

        internal static BucketsApi BucketsApi 
        {
            get
            {
                dynamic oauth = OAuthController.GetInternal();
                _bucketsApi.Configuration.AccessToken = oauth.access_token;
                
                return _bucketsApi;
            }
        }

        #endregion

        #region Methods

        public static async Task<dynamic> CreateBucket(string bucketKey)
        {
            try
            {
                PostBucketsPayload bucketPayload = new PostBucketsPayload(bucketKey.ToLower(), null, PostBucketsPayload.PolicyKeyEnum.Persistent);
                dynamic bucket = await BucketsApi.CreateBucketAsync(bucketPayload, "US");

                return bucket;
            }
            catch
            {
                return null; //return null if the bucket can not be created or if the bucket already exists
            }
        }


        /// <summary>
        /// Return urn of an object within a bucket
        /// </summary>
        public static async Task<dynamic> GetObjectDetailsAsync(string bucketKey, string objectName)
        {
            try
            {
                dynamic objectDetails = await ObjectsApi.GetObjectDetailsAsync(bucketKey, objectName);

                //add the encoded urn to the object
                objectDetails.encodedURN = Utils.Base64Encode((string)objectDetails.objectId);

                return objectDetails;
            }
            catch
            {
                return null; //call to GetObjectDetailsAsync should fail if no object is found
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
                dynamic objectDetails = await ObjectsApi.GetObjectDetailsAsync(bucketKey, objectName);
                return Utils.Base64Encode((string)objectDetails.objectId);
            }
            catch
            {
                return null;
            }

        }

        #endregion


        #region API

        /// <summary>
        /// Return list of buckets 
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/buckets")]
        public async Task<IActionResult> GetBucketsAsync(string startAt)
        {
            try
            {
                // to simplify, let's return only the first 100 buckets
                dynamic buckets = await BucketsApi.GetBucketsAsync("US", 100);

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
                return BadRequest(new { Error = ex.ToString() });
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
                await BucketsApi.DeleteBucketAsync(bucketKey);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.ToString() });
            }
        }

        /// <summary>
        /// Return list of objects in a bucket
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/objects")]
        public async Task<IActionResult> GetObjectsAsync(string bucketKey)
        {
            try
            {
                // as we have the id (bucketKey), let's return all 
                dynamic objects = await ObjectsApi.GetObjectsAsync(bucketKey, 100);

                //format buckets into viewable format
                List<ObjectDetails> objectsList = new List<ObjectDetails>();
                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objects.items))
                {
                    string urn = Utils.Base64Encode((string)objInfo.Value.objectId);
                    string key = objInfo.Value.objectKey;

                    ObjectDetails objectDetails = new ObjectDetails(bucketKey, urn, key);
                    objectsList.Add(objectDetails);
                }

                return Ok(new { Objects = objectsList });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.ToString() });
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
                dynamic resource = await ObjectsApi.CreateSignedResourceAsync(bucketKey, objectKey, new PostBucketsSigned(10), "read");

                return Ok(new { Url = (string)resource.signedUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.ToString() });
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
                //validate data
                if (string.IsNullOrWhiteSpace(fileUploadData.bucketKey)) throw new Exception("bucketKey parameter was not provided.");
                if (string.IsNullOrWhiteSpace(fileUploadData.objectName)) throw new Exception("objectName parameter was not provided.");
                if (!_uploadFileExtensions.Any(i => i == Path.GetExtension(fileUploadData.file.FileName))) throw new Exception("Invalid file type provided.");
                if (fileUploadData.file.Length == 0) throw new Exception("File does not contain any data.");

                string bucketKey = fileUploadData.bucketKey.ToLower();
                string objectKey = fileUploadData.objectName;

                //// Upload check if less than 2mb!
                //if (memoryStream.Length < 2097152)
                //{
                //}
                //else
                //{
                //}

                //create output bucket (in case is 't exist) 
                await CreateBucket(bucketKey);

                //void onUploadProgress(float progress, TimeSpan elapsed, List<UploadItemDesc> objects)
                //{
                //    Console.WriteLine("progress: {0} elapsed: {1} objects: {2}", progress, elapsed, string.Join(", ", objects));
                //}

                //method to refresh the token if it expires during upload
                static Task<Bearer> onRefreshToken()
                {
                    dynamic token = OAuthController.GetInternal();
                    //TRY TO FIGURE OUT HOW TO RETURN BEARER HERE
                    return null;
                }

                //set options for upload
                var uploadOptions = new Dictionary<string, object>()
                {
                    //{ "chunkSize", 3 }, // use 3Mb to make it fails, use a debug ApiClient, objectsApi.apiClient.isDebugMode = true
                    //{ "minutesExpiration", 60 }, // use 1 to stress error code 403 - Forbidden
                    //{ "Timeout", Timeout.InfiniteTimeSpan } // TimeSpan.FromSeconds (100) }
                    { "minutesExpiration", 5 },
                    { "useAcceleration", true }
                };


                //get temp file name to load file to the server
                string tempFileName = Path.GetTempFileName();

                //save the file to server
                using (FileStream stream = System.IO.File.Create(tempFileName))
                {
                    await fileUploadData.file.CopyToAsync(stream);
                }

                //upload to aws using direct to s3 approach
                UploadItemDesc result;
                using (StreamReader reader = new StreamReader(tempFileName))
                {
                    var uploadRes = await ObjectsApi.uploadResources(bucketKey: bucketKey,
                                                                     objects: Utils.CreateSingleUploadList(objectKey, reader.BaseStream),
                                                                     opts: uploadOptions,
                                                                     onUploadProgress: null, //onUploadProgress
                                                                     onRefreshToken: onRefreshToken);
                    result = uploadRes.First();
                }

                //delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                //translate the model if upload successful
                //if (!result.Error)
                //{
                //}

                //return the result
                return Ok(new { BucketKey = bucketKey, ObjectName = objectKey, Error = result.Error.ToString(), Response = result.completed.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Failed to upload object to Forge: {ex}" });
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
                await ObjectsApi.DeleteObjectAsync(bucketKey, objectKey);

                return Ok(new { Result = objectKey + " successfully deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.ToString() });
            }
        }

        #endregion






    }
}

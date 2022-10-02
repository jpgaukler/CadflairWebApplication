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
                await appBuckets.DeleteBucketAsync(bucketKey);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
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

                await objects.DeleteObjectAsync(bucketKey, objectKey);

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

        private static readonly string[] _validFileTypesForUpload = { ".ipt", "iam", ".idw", ".dwg" };

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
                if (!_validFileTypesForUpload.Any(i => i == Path.GetExtension(fileUploadData.file.FileName))) throw new Exception("Invalid file type provided.");
                if (fileUploadData.file.Length == 0) throw new Exception("File does not contain any data.");


                // get the auth token
                dynamic oauth = await OAuthController.GetInternalAsync();
                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = oauth.access_token;

                //create output buckets (in case they don't exist) 
                await OSSController.CreateBucket(fileUploadData.bucketKey);

                async Task<Bearer> onRefreshToken()
                {
                    //ApiResponse<dynamic>? bearer = await oauthExecAsync();
                    //// Note our oauthExecAsync method already updates the API wrappers
                    //// returning the bearer isn't stricly required (we could have returned null)
                    //return (new Bearer(bearer));

                    dynamic oauth = await OAuthController.GetInternalAsync();
                    objects.Configuration.AccessToken = oauth.access_token;
                    return null;
                }

                //void onUploadProgress(float progress, TimeSpan elapsed, List<UploadItemDesc> objects)
                //{
                //    Console.WriteLine("progress: {0} elapsed: {1} objects: {2}", progress, elapsed, string.Join(", ", objects));
                //}

                //read the file to memory
                using MemoryStream stream = new MemoryStream();
                await fileUploadData.file.CopyToAsync(stream);

                //// Upload check if less than 2mb!
                //if (memoryStream.Length < 2097152)
                //{
                //}
                //else
                //{
                //}

                var uploadList = new List<UploadItemDesc>
                {
                    new UploadItemDesc (fileUploadData.file.FileName, stream)

                    //new UploadItemDesc (FILE_NAME0, "this is a string"), // string test
					//new UploadItemDesc (FILE_NAME1 + ".txt", _buffer.ToString ()), // file:// test, we know it is a text file
					//new UploadItemDesc (FILE_NAME1, _buffer), // file:// test, but as Buffer this time
					//new UploadItemDesc (FILE_NAME1 + ".bin", _stream), // file:// test, but as ReadableStream this time
                };

                var uploadOptions = new Dictionary<string, object>()
                {
                    //{ "chunkSize", 3 }, // use 3Mb to make it fails, use a debug ApiClient, objectsApi.apiClient.isDebugMode = true
                    { "minutesExpiration", 2 },
                    { "useAcceleration", true }
                };
                    
                var uploadRes = await objects.uploadResources(bucketKey: fileUploadData.bucketKey,
                                                              objects: uploadList,
                                                              opts: uploadOptions,
                                                              onUploadProgress: null, //onUploadProgress
                                                              onRefreshToken: onRefreshToken);

                UploadItemDesc result = uploadRes.First();

                return Ok(new { BucketKey = fileUploadData.objectName, ObjectName = fileUploadData.objectName, Error = result.Error.ToString(), Response = result.completed.ToString()});

                ////report progress
                //Console.WriteLine("**** Upload object(s) response(s):");

                //foreach (var resp in uploadRes) Console.WriteLine("{0} {1}{2}", resp.objectKey, resp.Error ? "Error: " : "", resp.completed.ToString());

                ////verify SHA1 codes
                //Console.WriteLine("**** Verifying SHA1 codes"); // re-assembling files takes times, but we uploaded these files in 1 part :)

                //await verifyServerObjectsSha1(BucketKey, uploadRes);

                //           // Large Files < 5Mb <
                //           Console.WriteLine("**** Testing Large files");

                //           _buffer = File.ReadAllBytes(FILE_NAME2);
                //           _stream = File.Open(FILE_NAME2, FileMode.Open);

                //           byte[] _buffer3 = File.ReadAllBytes(FILE_NAME3);
                //           FileStream _stream3 = File.Open(FILE_NAME3, FileMode.Open);
                //           long size = _buffer3.LongLength;


                //           uploadRes = await ObjectsAPI.uploadResources(
                //               BucketKey,
                //               new List<UploadItemDesc>() {
                //               new UploadItemDesc (FILE_NAME2, _buffer),
                //               new UploadItemDesc (FILE_NAME2 + ".bin", _stream),
                //               new UploadItemDesc (FILE_NAME3, _buffer3),
                //               new UploadItemDesc (FILE_NAME3 + ".bin", _stream3),
                //               },
                //               new Dictionary<string, object>() {
                ////{ "chunkSize", 3 }, // use 3Mb to make it fails, use a debug ApiClient, objectsApi.apiClient.isDebugMode = true
                //{ "minutesExpiration", 60 }, // use 1 to stress error code 403 - Forbidden
                //{ "Timeout", Timeout.InfiniteTimeSpan } // TimeSpan.FromSeconds (100) }
                //               },
                //               onUploadProgress
                //           );


                //           Console.WriteLine("**** Upload file(s) response(s):");
                //           foreach (var resp in uploadRes) Console.WriteLine("{0} {1}{2}", resp.objectKey, resp.Error ? "Error: " : "", resp.completed.ToString());

                //           Console.WriteLine("**** Verifying SHA1 codes (please wait, the system is reassembling parts)");
                //           await verifyServerObjectsSha1(BucketKey, uploadRes);


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

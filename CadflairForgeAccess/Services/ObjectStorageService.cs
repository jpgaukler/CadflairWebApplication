using Autodesk.Forge;
using Autodesk.Forge.Model;
using CadflairForgeAccess.Helpers;
using RestSharp;
using System.Diagnostics;

namespace CadflairForgeAccess.Services
{
    public class ObjectStorageService
    {

        private readonly AuthorizationService _authService;
        //private static readonly string[] _uploadFileExtensions = { ".ipt", "iam", ".idw", ".dwg", ".zip" };

        public ObjectStorageService(AuthorizationService authService)
        {
            _authService = authService;
        }

        private async Task<BucketsApi> GetBucketsApi()
        {
            BucketsApi buckets = new();
            dynamic token = await _authService.GetInternal();
            buckets.Configuration.AccessToken = token.access_token;
            return buckets;
        }

        private async Task<ObjectsApi> GetObjectsApi()
        {
            ObjectsApi objects = new();
            dynamic token = await _authService.GetInternal();
            objects.Configuration.AccessToken = token.access_token;
            return objects;
        }

        public async Task<dynamic> CreateBucket(string bucketKey)
        {
            BucketsApi buckets = await GetBucketsApi();
            PostBucketsPayload bucketPayload = new(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Persistent);
            dynamic bucket = await buckets.CreateBucketAsync(bucketPayload, "US");

            return bucket;
        }

        public async Task DeleteBucket(string bucketKey)
        {
            BucketsApi buckets = await GetBucketsApi();
            await buckets.DeleteBucketAsync(bucketKey);
        }

        public async Task<List<string>> GetBucketKeys(string region = "US", int limit = 100, string? startAt = null)
        {
            BucketsApi bucketsApi = await GetBucketsApi();
            dynamic buckets = await bucketsApi.GetBucketsAsync(region, limit, startAt);

            List<string> bucketKeys = new();
            foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(buckets.items))
                bucketKeys.Add(bucket.Value.bucketKey);

            return bucketKeys;
        }

        public async Task<dynamic> GetBucketDetails(string bucketKey)
        {
            BucketsApi buckets = await GetBucketsApi();
            dynamic bucket = await buckets.GetBucketDetailsAsync(bucketKey);

            return bucket;
        }

        public async Task<dynamic> GetObjectDetails(string bucketKey, string objectKey)
        {
            ObjectsApi objects = await GetObjectsApi();
            dynamic objectDetails = await objects.GetObjectDetailsAsync(bucketKey, objectKey);

            //add the encoded urn to the object
            string objectId = (string)objectDetails.objectId;
            objectDetails.encoded_urn = objectId.ToBase64String();

            return objectDetails;
        }

        /// <summary>
        /// Upload a file directly to an AWS bucket on the Autodesk Forge service.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadFile(string bucketKey, string objectKey, string fullFileName)
        {
            bool success = false;

            try
            {
                ObjectsApi objects = await GetObjectsApi();

                // Method to report progress during the file upload
                void onUploadProgress(float progress, TimeSpan elapsed, List<UploadItemDesc> objects)
                {
                    //Debug.WriteLine($"File Upload Progress: {progress} Elapsed: {elapsed} Objects: {string.Join(", ", objects)}");
                }

                // Method to refresh the token if it expires during upload
                async Task<Bearer?> onRefreshToken()
                {
                    dynamic? token = await _authService.GetInternal();
                    return new Bearer(token.token_type, token.expires_in, token.access_token);
                }

                //set options for upload
                var uploadOptions = new Dictionary<string, object>()
                {
                    { "minutesExpiration", 10 },
                    { "useAcceleration", true }
                };

                // Upload to aws using direct to S3 approach
                using StreamReader reader = new(fullFileName);
                List<UploadItemDesc> uploadList = new() { new UploadItemDesc(objectKey, reader.BaseStream) };
                List<UploadItemDesc> uploadRes = await objects.uploadResources(bucketKey, uploadList, uploadOptions, onUploadProgress, onRefreshToken);
                UploadItemDesc result = uploadRes.First();

                //Debug.WriteLine($"File upload complete: {result.completedResponse}");

                success = !result.Error;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"File upload failed: {ex}");
                success = false;
            }

            return success;
        }

        public async Task<string> GetSignedDownloadUrl(string bucketKey, string objectKey, int minuteExpiration = 60, bool singleUse = true)
        {
            ObjectsApi objects = await GetObjectsApi();
            dynamic result = await objects.CreateSignedResourceAsync(bucketKey, objectKey, new PostBucketsSigned(minuteExpiration, singleUse), "read");
            return result.signedUrl;
        }

        public async Task<string> GetSignedUploadUrl(string bucketKey, string objectKey, int minuteExpiration = 60, bool singleUse = true)
        {
            ObjectsApi objects = await GetObjectsApi();
            dynamic result = await objects.CreateSignedResourceAsync(bucketKey, objectKey, new PostBucketsSigned(minuteExpiration, singleUse), "write");
            return result.signedUrl;
        }

        /// <summary>
        /// Retrieve a thumbnail image from an OSS bucket and convert it to a base64 string.
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public async Task<string> GetThumbnailAsBase64(string bucketKey, string objectKey = "thumbnail.png")
        {
            try
            {
                ObjectsApi objects = await GetObjectsApi();
                dynamic result = await objects.CreateSignedResourceAsync(bucketKey, objectKey, new PostBucketsSigned(15, true), "read");

                // download data and convert to base64
                RestClient client = new(result.signedUrl);
                byte[]? bytes = await client.DownloadDataAsync(new RestRequest());

                return bytes?.ToBase64String() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task DeleteObject(string bucketKey, string objectKey)
        {
            ObjectsApi objects = await GetObjectsApi();
            await objects.DeleteObjectAsync(bucketKey, objectKey);
        }

    }
}

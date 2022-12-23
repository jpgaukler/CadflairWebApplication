using Autodesk.Forge;
using Autodesk.Forge.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

        public async Task<dynamic> GetBucketDetails(string bucketKey)
        {
            BucketsApi buckets = await GetBucketsApi();
            dynamic bucket = await buckets.GetBucketDetailsAsync(bucketKey);

            return bucket;
        }

        public async Task<dynamic> GetObjectDetails(string bucketKey, string objectName)
        {
            ObjectsApi objects = await GetObjectsApi();
            dynamic objectDetails = await objects.GetObjectDetailsAsync(bucketKey, objectName);

            //add the encoded urn to the object
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes((string)objectDetails.objectId);
            objectDetails.encoded_urn = System.Convert.ToBase64String(plainTextBytes);

            return objectDetails;
        }

        /// <summary>
        /// Upload a file directly to an AWS bucket on the Autodesk Forge service.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadFile(string bucketKey, string objectKey, string fullFileName)
        {
            ObjectsApi objects = await GetObjectsApi();

            // Method to report progress during the file upload
            void onUploadProgress(float progress, TimeSpan elapsed, List<UploadItemDesc> objects)
            {
                Debug.WriteLine($"File Upload Progress: {progress} Elapsed: {elapsed} Objects: {string.Join(", ", objects)}");
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

            Debug.WriteLine($"File upload complete: {result.completedResponse}");

            bool success = !result.Error;
            return success;
        }
    }
}

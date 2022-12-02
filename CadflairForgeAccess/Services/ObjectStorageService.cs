using Autodesk.Forge;
using Autodesk.Forge.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CadflairForgeAccess.Services
{
    public class ObjectStorageService
    {

        private readonly AuthorizationService _authService;
        private static readonly ObjectsApi _objectsApi = new();
        private static readonly BucketsApi _bucketsApi = new();
        private static readonly string[] _uploadFileExtensions = { ".ipt", "iam", ".idw", ".dwg", ".zip" };

        public ObjectStorageService(AuthorizationService authService)
        {
            _authService = authService;
        }

        public async Task<dynamic?> CreateBucketAsync(string bucketKey)
        {
            dynamic token = await _authService.GetInternalAsync();
            _bucketsApi.Configuration.AccessToken = token.access_token;

            PostBucketsPayload bucketPayload = new(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Persistent);
            dynamic bucket = await _bucketsApi.CreateBucketAsync(bucketPayload, "US");

            return bucket;
        }

        public async Task<dynamic?> GetBucketDetailsAsync(string bucketKey)
        {
            dynamic token = await _authService.GetInternalAsync();
            _bucketsApi.Configuration.AccessToken = token.access_token;

            dynamic bucket = await _bucketsApi.GetBucketDetailsAsync(bucketKey);

            return bucket;
        }

        /// <summary>
        /// Upload a file directly to an AWS bucket on the Autodesk Forge service.
        /// </summary>
        /// <returns></returns>
        public async Task UploadFileAsync(string bucketKey, string objectKey, string fullFileName)
        {
            // Method to report progress during the file upload
            void onUploadProgress(float progress, TimeSpan elapsed, List<UploadItemDesc> objects)
            {
                Debug.WriteLine($"Progress: {progress} Elapsed: {elapsed} Objects: {string.Join(", ", objects)}");
            }

            // Method to refresh the token if it expires during upload
            async Task<Bearer?> onRefreshToken()
            {
                dynamic? token = await _authService.GetInternalAsync();
                //TRY TO FIGURE OUT HOW TO RETURN BEARER HERE
                return null;
            }

            //set options for upload
            var uploadOptions = new Dictionary<string, object>()
            {
                //{ "chunkSize", 3 }, // use 3Mb to make it fails, use a debug ApiClient, objectsApi.apiClient.isDebugMode = true
                //{ "minutesExpiration", 60 }, // use 1 to stress error code 403 - Forbidden
                //{ "Timeout", Timeout.InfiniteTimeSpan } // TimeSpan.FromSeconds (100) }
                { "minutesExpiration", 10 },
                { "useAcceleration", true }
            };

            // Upload to aws using direct to S3 approach
            using StreamReader reader = new(fullFileName);
            List<UploadItemDesc> uploadList = new() { new UploadItemDesc(objectKey, reader.BaseStream) };
            List<UploadItemDesc> uploadRes = await _objectsApi.uploadResources(bucketKey, uploadList, uploadOptions, onUploadProgress, onRefreshToken);
            UploadItemDesc result = uploadRes.First();

            Debug.WriteLine($"Response: {result.completedResponse} Error: {result.Error} BucketKey: {bucketKey} ObjectKey {objectKey} ");
        }
    }
}

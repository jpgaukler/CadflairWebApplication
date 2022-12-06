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
        public async Task<bool> UploadFileAsync(string bucketKey, string objectKey, string fullFileName)
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
            List<UploadItemDesc> uploadRes = await _objectsApi.uploadResources(bucketKey, uploadList, uploadOptions, onUploadProgress, onRefreshToken);
            UploadItemDesc result = uploadRes.First();

            Debug.WriteLine($"Response: {result.completedResponse}");

            bool success = !result.Error;
            return success;
        }
    }
}

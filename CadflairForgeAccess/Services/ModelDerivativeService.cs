using Autodesk.Forge;
using Autodesk.Forge.Model;
using CadflairForgeAccess.Helpers;

namespace CadflairForgeAccess.Services
{
    public class ModelDerivativeService
    {

        private readonly AuthorizationService _authService;
        private readonly ObjectStorageService _objectStorageService;

        public ModelDerivativeService(AuthorizationService authService, ObjectStorageService objectStorageService)
        {
            _authService = authService;
            _objectStorageService = objectStorageService;
        }

        private async Task<DerivativesApi> GetDerivativesApi()
        {
            DerivativesApi derivative = new();
            dynamic token = await _authService.GetInternal();
            derivative.Configuration.AccessToken = token.access_token;
            return derivative;
        }

        public async Task<bool> TranslationExists(string encodedUrn)
        {
            try
            {
                DerivativesApi derivative = await GetDerivativesApi();
                dynamic manifest = await derivative.GetManifestAsync(encodedUrn);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<dynamic> TranslateObject(string bucketKey, string objectKey, bool isZip = false, string? rootFileName = null, string? connectionId = null)
        {
            var objectDetails = await _objectStorageService.GetObjectDetails(bucketKey, objectKey);

            // configure output
            JobPayloadDestination destination = new(JobPayloadDestination.RegionEnum.US);
            JobPayloadItem.TypeEnum jobType = JobPayloadItem.TypeEnum.Svf;
            List<JobPayloadItem.ViewsEnum> views = new()
            {
                JobPayloadItem.ViewsEnum._2d,
                JobPayloadItem.ViewsEnum._3d
            };
            List<JobPayloadItem> formats = new()
            {
                new JobPayloadItem(jobType, views)
            };

            JobPayload job = new()
            {
                Input = new JobPayloadInput(objectDetails.encoded_urn, isZip, rootFileName),
                Output = new JobPayloadOutput(formats, destination)
            };

            if (connectionId != null)
            {
                // set up attributes for webhook (to provide a callback when the translation is complete)
                dynamic attribute = new
                {
                    connectionId,
                    bucketKey,
                    objectKey
                };

                job.Misc = new JobPayloadMisc("TranslationCompleted", attribute);
            }

            // start the translation
            DerivativesApi derivative = await GetDerivativesApi();
            dynamic jobPosted = await derivative.TranslateAsync(job);

            return jobPosted;
        }

        public async Task DeleteTranslation(string encodedUrn)
        {
            DerivativesApi derivative = await GetDerivativesApi();
            await derivative.DeleteManifestAsync(encodedUrn);
        }



        /// <summary>
        /// Get the thumbnail for a model in Forge OSS storage
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="objectKey"></param>
        /// <param name="width">The desired width of the thumbnail. Possible values are 100, 200 and 400.  (optional)</param>
		/// <param name="height">The desired height of the thumbnail. Possible values are 100, 200 and 400.  (optional)</param>
        /// <returns></returns>
        public async Task<string> GetThumbnailBase64String(string bucketKey, string objectKey, int width = 100, int height = 100)
        {
            try
            {
                var objectDetails = await _objectStorageService.GetObjectDetails(bucketKey, objectKey);

                DerivativesApi derivative = await GetDerivativesApi();
                Stream thumbnailStream = await derivative.GetThumbnailAsync(objectDetails.encoded_urn, width, height);

                byte[] byteArray;

                using (var memoryStream = new MemoryStream())
                {
                    await thumbnailStream.CopyToAsync(memoryStream);
                    byteArray = memoryStream.ToArray();
                }

                return byteArray.ToBase64String();
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

    }
}

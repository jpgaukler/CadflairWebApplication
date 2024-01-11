using CadflairDataAccess;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Mvc;

namespace CadflairRestApi.Controllers;

[ApiController]
[Route("api/v1")]
public class ForgeController : ControllerBase
{
    private readonly ForgeServicesManager _forgeServicesManager;
    private readonly DataServicesManager _dataServicesManager;
    private readonly ILogger<ForgeController> _logger;

    public ForgeController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager, ILogger<ForgeController> logger)
    {
        _forgeServicesManager = forgeServicesManager;
        _dataServicesManager = dataServicesManager;
        _logger = logger;
    }

    //[HttpGet]
    //[Route("forge/viewer/token")]
    //public async Task<IActionResult> GetPublicForgeToken()
    //{
    //    try
    //    {
    //        var token = await _forgeServicesManager.AuthorizationService.GetPublic();
    //        return Ok(token);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"An unknown error occurred!");
    //        return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
    //    }
    //}

    [HttpGet]
    [Route("forge/buckets/{bucketKey}/objects/{objectKey}/urn")]
    public async Task<IActionResult> GetForgeObjectUrn(string bucketKey, string objectKey)
    {
        try
        {
            // get token for viewer
            var token = await _forgeServicesManager.AuthorizationService.GetPublic();

            // get forge object id
            dynamic forgeObject = await _forgeServicesManager.ObjectStorageService.GetObjectDetails(bucketKey, objectKey);

            if (await _forgeServicesManager.ModelDerivativeService.TranslationExists(forgeObject.encoded_urn))
            {
                return Ok(new
                {
                    Token = token.access_token,
                    Urn = forgeObject.encoded_urn
                });
            }
            else
            {
                _logger.LogWarning($"Model derivative manifest not found - bucketKey: {bucketKey}, objectKey: {objectKey}");
                return NotFound($"Model derivative manifest not found - bucketKey: {bucketKey}, objectKey: {objectKey}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to load model from derivative - bucketKey: {bucketKey}, objectKey: {objectKey}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to load model from derivative - bucketKey: {bucketKey}, objectKey: {objectKey}");
        }
    }
}

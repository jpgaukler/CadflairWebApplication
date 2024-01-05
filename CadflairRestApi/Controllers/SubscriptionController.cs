using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairDataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadflairRestApi.Controllers;

[ApiController]
[Route("api/v1")]
public class SubscriptionController : ControllerBase
{

    private readonly SubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(DataServicesManager dataServicesManager, ILogger<SubscriptionController> logger)
    {
        _subscriptionService = dataServicesManager.SubscriptionService;
        _logger = logger;
    }

    //[HttpPost]
    //[Route("subscriptions")]
    //public Task<ActionResult<Subscription>> CreateSubscription(int subscriptionTypeId, string companyName, int ownerId, int createdById)
    //{
    //}

    //[HttpGet]
    //[Route("subscriptions")]
    //public Task<ActionResult<List<SubscriptionType>>> GetSubscriptionTypes()
    //{
    //}

    [HttpGet]
    [Route("subscriptions/{id:int}")]
    public async Task<ActionResult<Subscription>> GetSubscriptionById(int id)
    {
        try
        {
            var result = await _subscriptionService.GetSubscriptionById(id);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    [HttpGet]
    [Route("subscriptions/{subdirectoryName}")]
    public async Task<ActionResult<Subscription>> GetSubscriptionBySubdirectoryName(string subdirectoryName)
    {
        try
        {
            var result = await _subscriptionService.GetSubscriptionBySubdirectoryName(subdirectoryName);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    [HttpDelete]
    [Route("subscriptions/{id}")]
    public async Task<IActionResult> DeleteSubscription(int id)
    {
        try
        {
            await _subscriptionService.DeleteSubscriptionById(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }
}

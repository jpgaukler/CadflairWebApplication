using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace CadflairBlazorServer.Controllers
{
    [Authorize]
    [RequiredScope("User.InventorAddin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataServicesManager _dataServicesManager;
        private readonly ILogger<UserController> _logger;

        public UserController(DataServicesManager dataServicesManager, ILogger<UserController> logger)
        {
            _dataServicesManager = dataServicesManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/v1/users/{objectIdentifier}")]
        public async Task<IActionResult> GetUserByObjectIdentifier(string objectIdentifier)
        {
            try
            {
                User user = await _dataServicesManager.UserService.GetUserByObjectIdentifier(objectIdentifier);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"{ex}" });
            }
        }
    }
}

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

        public UserController(DataServicesManager dataServicesManager)
        {
            _dataServicesManager = dataServicesManager;
        }

        [HttpGet]
        [Route("api/v1/user/get/{objectIdentifier}")]
        public async Task<IActionResult> GetUserByObjectIdentifier(string objectIdentifier)
        {
            try
            {
                User user = await _dataServicesManager.UserService.GetUserByObjectIdentifier(objectIdentifier);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"{ex}" });
            }
        }
    }
}

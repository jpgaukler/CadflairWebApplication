using CadflairDataAccess;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CadflairRestApi.Controllers;

[ApiController]
[Route("api/v1")]
public class EmailController : ControllerBase
{
    private readonly DataServicesManager _dataServicesManager;
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<EmailController> _logger;

    public EmailController(DataServicesManager dataServicesManager, IFluentEmail fluentEmail, ILogger<EmailController> logger)
    {
        _dataServicesManager = dataServicesManager;
        _fluentEmail = fluentEmail;
        _logger = logger;
    }


    public class ContactUsForm
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required]
        public string EmailAddress { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public string Message { get; set; } = default!;
    }


    [HttpPost]
    [Route("contact-us")]
    public async Task<IActionResult> ContactUs(ContactUsForm form)
    {
        try
        {
            string bodyText = $"""
                              I want to know more about Cadflair. Please add me to your mailing list!

                              Name: {form.FirstName} {form.LastName}
                              Email: {form.EmailAddress} 
                              Company: {form.CompanyName} 

                              Message: 
                              {form.Message} 
                              """;

            await _dataServicesManager.NotificationService.CreateContactRequest(form.FirstName, form.LastName, form.EmailAddress, form.CompanyName, form.Message);
            string toAddress = "justin.gaukler@verizon.net";

            SendResponse email = await _fluentEmail.SetFrom("donotreply@cadflair.com")
                                                    .To(toAddress)
                                                    .Subject("Contact Us Request!")
                                                    .Body(bodyText)
                                                    .SendAsync();

            if (email.Successful)
            {
                _logger.LogInformation($"Email sent successfully. Email Address: {toAddress}");
                return Ok();
            }
            else
            {
                email.ErrorMessages.ForEach(message => _logger.LogError($"Email failed to send.  Email Address: {toAddress} Error Message: {message}"));
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send message!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ContactUs)} failed: {ex}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send message!");
        }
    }

}

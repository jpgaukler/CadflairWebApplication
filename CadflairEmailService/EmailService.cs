using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.MailKitSmtp;
using FluentEmail.Razor;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairEmailService
{
    public class EmailService
    {
        private MailKitSender _sender;

        public EmailService(IConfiguration configuration)
        {
            SmtpClientOptions options = new()
            {
                User = "jpgaukler@gmail.com",
                Password = "password",
                UseSsl = true,
                RequiresAuthentication = true,
                Server = "smtp.gmail.com",
                Port = 587,
                SocketOptions = SecureSocketOptions.StartTls
            };

            _sender = new(options);

            Email.DefaultSender = _sender;
            Email.DefaultRenderer = new RazorRenderer();
        }


        public async Task SendEmail()
        {

            StringBuilder template = new();
            template.AppendLine("Dear @Model.FirstName,");
            template.AppendLine("<p>Thanks for purchasing @Model.ProductName. We hope you enjoy it.</p>");
            template.AppendLine("- Cadflair");


            SendResponse email = await Email.From("donotreply@cadflair.com")
                                   .To("justin.gaukler@verizon.net", "Justin Gaukler")
                                   .Subject("Thanks!")
                                   //.Body("Test message")
                                   .UsingTemplate(template.ToString(), new { FirstName = "Justin", ProductName = "Cadflair Pro" })
                                   .SendAsync();

            Debug.WriteLine($"MessageId: {email.MessageId}, Successful: {email.Successful}");

            foreach(string error in email.ErrorMessages)
            {
                Debug.WriteLine($"Error: {error}");
            }
        }

        //public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        //{
        //    SmtpClientOptions options = new()
        //    {
        //        User = "test",
        //        Password = "passweord",
        //        UseSsl = true,
        //        RequiresAuthentication = true,
        //        Server = "smtp.office365.com",
        //        Port = 587,
        //        SocketOptions = SecureSocketOptions.StartTls
        //    };

        //    services.AddFluentEmail("fromemail@test.test")
        //            .AddRazorRenderer()
        //            .AddMailKitSender(options);

        //    return services;
        //}


    }
}

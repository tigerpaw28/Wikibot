using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Razor;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Wikibot.App.Services
{
    public class SmtpSender : IEmailSender
    {
        public SmtpSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
            var sender = new FluentEmail.Smtp.SmtpSender(() => new SmtpClient(Options.SmtpUrl)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 25
            });

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();
        }

        public EmailSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
             return Email
                .From(Options.FromName)
                .To(email)
                .Subject(subject)
                .Body(htmlMessage, true)
                .SendAsync();
        }
    }
}

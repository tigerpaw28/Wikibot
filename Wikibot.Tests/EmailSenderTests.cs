using FluentEmail.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikibot.App.Services;
using Xunit;
using Xunit.Abstractions;

namespace Wikibot.Tests
{
    public class EmailSenderTests
    {
        private ITestOutputHelper _output;
        public EmailSenderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SendEmailUsingSMTP()
        {
            var config = Utilities.GetIConfigurationRoot();

            EmailSenderOptions options = new EmailSenderOptions();
            var section = config.GetSection("EmailSenderOptions");
                section.Bind(options);
            IOptions<EmailSenderOptions> iOptions = Options.Create(options);
            var sender = new SmtpSender(iOptions);
            await sender.SendEmailAsync("test@test.com", "New request notification", "You have a new request to review.");
        }
    }
}

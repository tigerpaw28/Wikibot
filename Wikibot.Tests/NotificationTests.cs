using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikibot.DataAccess;
using Wikibot.Logic;
using Wikibot.Logic.Logic;
using Xunit;
using Xunit.Abstractions;

namespace Wikibot.Tests
{
    public class NotificationTests
    {
        private ITestOutputHelper _output;
        public NotificationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SendNewRequestNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = "ItsRunny", RequestComment = "Need to replace some links" };
            var users = new List<User>() { new User { Username = config.GetValue<string>("RootAdminUserName")} };
            var result = await emailNotification.SendNewRequestNotification(users, model.RequestUser, model.RequestComment);
            Assert.All(result, x => Assert.True(x.IsSuccess));
        }

        [Fact]
        public async Task SendNewApprovalNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = "ItsRunny", RequestComment = "Need to replace some links" };
            var users = new List<User>() { new User { Username = config.GetValue<string>("RootAdminUserName") } };
            var result = await emailNotification.SendNewApprovalNotification(users, model.RequestUser, model.RequestComment);
            Assert.All(result, x => Assert.True(x.IsSuccess));
        }

        [Fact]
        public async Task SendErrorNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = "ItsRunny", RequestComment = "Need to replace some links" };
            var users = new List<User>() { new User { Username = config.GetValue<string>("RootAdminUserName") } };
            var result = await emailNotification.SendErrorNotification(users, model.RequestUser, model.RequestComment);
            Assert.All(result, x => Assert.True(x.IsSuccess));
        }

        [Fact]
        public async Task SendRequestCompletedNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = config.GetValue<string>("RootAdminUserName"), RequestComment = "Need to replace some links" };
            var result = await emailNotification.SendRequestCompletedNotification(model.RequestUser, model.RequestComment, "LinkFix");
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task SendRequestPreApprovedNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = config.GetValue<string>("RootAdminUserName"), RequestComment = "Need to replace some links" };
            var result = await emailNotification.SendRequestPreApprovedNotification(model.RequestUser, model.RequestComment, "LinkFix");
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task SendRequestApprovedNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = config.GetValue<string>("RootAdminUserName"), RequestComment = "Need to replace some links" };
            var result = await emailNotification.SendRequestApprovedNotification(model.RequestUser, model.RequestComment, "LinkFix");
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task SendRequestRejectedNotification()
        {
            var config = Utilities.GetIConfigurationRoot();

            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig, _output);
            var wikiAccessLogic = new WikiAccessLogic(iConfig, logger);
            var emailNotification = new WikiNotificationService(wikiAccessLogic, iConfig);
            var model = new { RequestUser = "ItsRunny", RequestComment = "Need to replace some links" };
            var result = await emailNotification.SendRequestRejectedNotification(config.GetValue<string>("RootAdminUserName"), model.RequestComment, "LinkFix",
                "Too many incorrect fixes showing up. We need to fix the definition.", model.RequestUser);
            Assert.True(result.IsSuccess);
        }
    }
}

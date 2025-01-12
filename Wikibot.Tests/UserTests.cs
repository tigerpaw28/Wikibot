using System.Collections.Generic;
using Wikibot.DataAccess;
using Wikibot.Logic.Logic;
using Xunit;
using Xunit.Abstractions;

namespace Wikibot.Tests
{
    public class UserTests
    {
        private ITestOutputHelper _output;
        public UserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GetUser()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var wikiLoginConfig = iConfig.GetSection("WikiLogin");
            var username = wikiLoginConfig["Username"];
            var userRetriever = Utilities.GetUserRetriever(iConfig, Utilities.GetLogger(iConfig, _output));
            var user = userRetriever.GetUser(username);
            Assert.NotNull(user);
        }

        [Fact]
        public void GetAutoApprovedUsers()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var userRetriever = Utilities.GetUserRetriever(iConfig, Utilities.GetLogger(iConfig, _output));
            var users = userRetriever.GetAutoApprovedUsers();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public void GetReviewerUsers()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var userRetriever = Utilities.GetUserRetriever(iConfig, Utilities.GetLogger(iConfig, _output));
            List<User> reviewers = userRetriever.GetReviewerUsers();
  
            Assert.NotNull(reviewers);
            Assert.NotEmpty(reviewers);
        }

        [Fact]
        public void SysOpUserIsApproved()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var userRetriever = Utilities.GetUserRetriever(iConfig, Utilities.GetLogger(iConfig, _output));
            var jobApprovalLogic = new JobApprovalLogic(userRetriever);
            var user = userRetriever.GetUser(iConfig["ExampleSysOp"]);
            Assert.True(jobApprovalLogic.IsUserAutoApproved(user));
        }
    }
}

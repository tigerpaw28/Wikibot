using Xunit;

namespace Wikibot.Tests
{
    public class UserTests
    {
        [Fact]
        public void GetUser()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var userRetriever = Utilities.GetUserRetriever(iConfig);
            var user = userRetriever.GetUser("Tigerpaw28");
            Assert.NotNull(user);
        }

        [Fact]
        public void GetAutoApprovedUsers()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var userRetriever = Utilities.GetUserRetriever(iConfig);
            var users = userRetriever.GetAutoApprovedUsers();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }
    }
}

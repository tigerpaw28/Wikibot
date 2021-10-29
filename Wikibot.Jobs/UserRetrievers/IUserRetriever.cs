using System.Collections.Generic;
using Wikibot.DataAccess;

namespace Wikibot.Logic.UserRetrievers
{
    public interface IUserRetriever
    {
        public List<User> GetAutoApprovedUsers();

        public User GetUser(string username);

        public List<User> GetReviewerUsers();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikibot.App.Models.UserRetrievers
{
    public interface IUserRetriever
    {
        public List<User> GetAutoApprovedUsers();

        public User GetUser(string username);
    }
}

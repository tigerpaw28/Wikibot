using LinqToWiki.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Wikibot.App.Models.UserRetrievers
{
    public class TFWikiUserRetriever : IUserRetriever
    {
        private Wiki _wiki;
        public TFWikiUserRetriever(Wiki wiki)
        {
            _wiki = wiki;
        }
        public List<User> GetAutoApprovedUsers()
        {
            return (from user in _wiki.Query.allusers()
                         where user.rights == allusersrights.undelete
                         orderby user descending
                         select new User { Username = user.name, UserId = user.userid })
                         .ToEnumerable().ToList();
        }

        public User GetUser(string username)
        {
            return _wiki.Query.allusers().Where(user => user.activeusers).ToEnumerable().Where(x => x.name.Equals(username))
                    .Select(user => new User { Username = user.name, UserId = user.userid }).SingleOrDefault();
        }
    }
}

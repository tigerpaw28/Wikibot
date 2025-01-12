using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.Logic.Logic;

namespace Wikibot.Logic.UserRetrievers
{
    public class TFWikiUserRetriever : IUserRetriever
    {
        private Wiki _wiki;
        private IWikiAccessLogic _accessLogic;
        private string _sysAdminUserName;
        private string _reviewerGroupName;

        public TFWikiUserRetriever(IWikiAccessLogic accessLogic, IConfiguration config)
        {
            _accessLogic = accessLogic;
            _wiki = accessLogic.GetLoggedInWiki();
            _reviewerGroupName = config.GetValue<string>("ReviewerGroupName");
            _sysAdminUserName = config.GetValue<string>("RootAdminUsername");
        }
        public List<User> GetAutoApprovedUsers()
        {
            var list = (from user in _wiki.Query.allusers()
                    where user.@group == allusersgroup.sysop
                    orderby user descending
                    select new User { Username = user.name, UserId = user.userid })
                         .ToEnumerable().ToList();

            if (!list.Any(x=> x.Username == _sysAdminUserName))
            {
                list.Add(GetUser(_sysAdminUserName));
            }

            return list;
        }

        public User GetUser(string username)
        {
            return _wiki.Query.allusers().ToEnumerable().Where(x => x.name.Equals(username))
                    .Select(user => new User { Username = user.name, UserId = user.userid }).SingleOrDefault();
        }

        public List<User> GetReviewerUsers()
        {   
            var list = new List<User>();
            if (_reviewerGroupName != null)
            {
                list = (from user in _wiki.Query.allusers()
                            where user.@group == allusersgroup.sysop //Temp value until actual reviewer group is created
                            orderby user descending
                            select new User { Username = user.name, UserId = user.userid })
                             .ToEnumerable().ToList();
            }
            if (!list.Any(x => x.Username == _sysAdminUserName))
            {
                list.Add(GetUser(_sysAdminUserName));
            }

            return list;
        }
    }
}

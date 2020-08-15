using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Models;
using Wikibot.App.Models.UserRetrievers;

namespace Wikibot.App.Logic
{
    public class JobApprovalLogic
    {
        private IUserRetriever _userRetriever;
        public JobApprovalLogic(IUserRetriever userRetriever)
        {
            _userRetriever = userRetriever;
        }
        public bool IsUserAutoApproved(User userToValidate)
        {
            return userToValidate.Username.Equals("Tigerpaw28") || _userRetriever.GetAutoApprovedUsers().Where(user => user == userToValidate).Any();
        }

        public bool IsUserAuthentic(User userToValidate, string pageName)
        {
            bool isAuthentic = false;

            return isAuthentic;
        }
    }
}

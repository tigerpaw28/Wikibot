﻿using System;
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
            return _userRetriever.GetAutoApprovedUsers().Where(user => user == userToValidate).Any();
        }
    }
}

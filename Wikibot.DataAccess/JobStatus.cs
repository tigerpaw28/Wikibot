using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikibot.DataAccess
{
    public enum JobStatus
    {
        ToBeProcessed,
        PendingPreApproval,
        PreApproved,
        PendingApproval,
        Approved,
        Rejected,
        Done,
        Failed
    }
}

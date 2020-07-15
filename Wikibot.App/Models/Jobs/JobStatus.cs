using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikibot.App.Jobs
{
    public enum JobStatus
    {
        ToBeProcessed,
        ScheduledForProcessing,
        Processing,
        PendingApproval,
        Approved,
        Rejected,
        ScheduledForPosting,
        Done
    }
}

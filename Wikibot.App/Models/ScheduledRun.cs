using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Jobs;

namespace Wikibot.App.Models
{
    public class ScheduledRun
    {
        public WikiJob Job { get; set; }
        public DateTime ScheduledRunTime { get; set; }
        public string ChangesFileName { get; set; }
        //public JobResult Result { get; set; }
        public List<string> Messages { get; set; } //Make part of result object.
    }
}

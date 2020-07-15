using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Jobs;

namespace Wikibot.App.Jobs
{
    public class TextReplacementJob: AbstractJob
    {

        public string FromText { get; set; }
        public string ToText { get; set; }
        public List<Page> PageNames { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}

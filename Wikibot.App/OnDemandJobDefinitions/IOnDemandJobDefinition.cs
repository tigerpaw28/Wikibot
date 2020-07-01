using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikibot.App.JobDefinitions
{
    public interface IOnDemandJobDefinition
    {
        public List<string> PageNames { get; set; }


        public void RunJob();
    }
}

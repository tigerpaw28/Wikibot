using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using Wikibot.App.Jobs;

namespace Wikibot.App.JobRetrievers
{
    public interface IWikiJobRetriever
    {
        public List<WikiJob> JobDefinitions { get; }
        public Task<List<WikiJob>> GetNewJobDefinitions();

        public void MarkJobStatuses(List<WikiJob> jobs);

    }
}
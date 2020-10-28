using System.Collections.Generic;
using System.Threading.Tasks;
using Wikibot.DataAccess.Objects;

namespace Wikibot.Logic.JobRetrievers
{
    public interface IWikiJobRetriever
    {
        public List<WikiJobRequest> JobDefinitions { get; }
        public Task<List<WikiJobRequest>> GetNewJobDefinitions();
        public void UpdateRequests(List<WikiJobRequest> jobs);

    }
}
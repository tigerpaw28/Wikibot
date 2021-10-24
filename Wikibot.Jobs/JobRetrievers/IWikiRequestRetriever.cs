using System.Collections.Generic;
using System.Threading.Tasks;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Jobs;

namespace Wikibot.Logic.JobRetrievers
{
    public interface IWikiRequestRetriever
    {
        public List<WikiJobRequest> RequestDefinitions { get; }

        WikiJob GetJobForRequest(WikiJobRequest request);
        public Task<List<WikiJobRequest>> GetNewJobDefinitions();
        public void UpdateRequests(List<WikiJobRequest> jobs);

    }
}
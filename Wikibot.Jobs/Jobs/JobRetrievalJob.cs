using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Factories;
using Wikibot.Logic.JobRetrievers;
using Wikibot.Logic.Logic;
using Wikibot.Logic.UserRetrievers;

namespace Wikibot.Logic.Jobs
{
    public class JobRetrievalJob : AbstractJob
    {

        private IWikiJobRetriever _jobRetriever;
        private IUserRetriever _userRetriever;
        private IWikiAccessLogic _wikiAccessLogic;

        public JobRetrievalJob(IConfiguration config, Serilog.ILogger log, IWikiJobRetriever jobRetriever, IWikiAccessLogic wikiAccessLogic, RequestData jobData)
        {
            Configuration = config;
            Log = log;
            _jobRetriever = jobRetriever;
            var wikiConfig = Configuration.GetSection("WikiLogin");
            _wikiAccessLogic = wikiAccessLogic;
            var wiki = _wikiAccessLogic.GetLoggedInWiki(wikiConfig);
            _userRetriever = new TFWikiUserRetriever(wiki);

            JobData = jobData;
        }

        public override void Execute() {
            Log.Information("Job retrieval job starting");

            var jobApprovalLogic = new JobApprovalLogic(_userRetriever);
            int offset = 1;
            int runin = 5;

            //Get job definitions
             var requests = _jobRetriever.JobDefinitions;

            //using (JobContext _context = new JobContext(DBOptions))
            //{
                foreach (WikiJobRequest request in requests)
                {
                    //Check For Automatic Approval
                    CheckForUserApproval(request, jobApprovalLogic);

                    //Save Job
                    JobData.SaveWikiJobRequest(request);

                    //Set JobID so we have it available when the job runs
                    //request.ID = DBContext.Jobs.AsEnumerable().Last().ID;


                    if (request.Status == JobStatus.Approved || request.Status == JobStatus.PreApproved)
                    {
                        //Schedule jobs in 5 minute intervals
                        //How to deal with potential page edit overlaps? -> Check page lists and id overlaps;
                        if (offset == 5)
                        {
                            runin = runin + offset;
                            offset = 0;
                        }
                        var job = WikiJobFactory.GetWikiJob(request, Log, _wikiAccessLogic, Configuration, JobData);
                        JobManager.AddJob(() => job.Execute(), (s) => s.ToRunOnceIn(runin).Minutes());
                        offset++;
                    }
                }  
           //

            //Update job status on the wiki page the job was retreived from
            _jobRetriever.MarkJobStatuses(requests);

        }

        private void CheckForUserApproval(WikiJobRequest request, JobApprovalLogic jobApprovalLogic)
        {
            var user = _userRetriever.GetUser(request.Username);

            if (jobApprovalLogic.IsUserAutoApproved(user))
            {
                request.Status = JobStatus.PreApproved;
            }
            else
            {
                request.Status = JobStatus.PendingPreApproval;
            }
        }
    }
}